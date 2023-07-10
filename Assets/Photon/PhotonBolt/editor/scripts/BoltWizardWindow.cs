using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Bolt.Editor.Utils;
using Bolt.Utils;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using EditorUtility = UnityEditor.EditorUtility;

[InitializeOnLoad]
public partial class BoltWizardWindow : EditorWindow
{
    BoltSetupStage currentStage = BoltSetupStage.Intro;
    static Stopwatch watch = new Stopwatch();

    static Boolean? Ready;
    static Single? FirstCall;
    private static volatile bool requestingAppId = false;

    static bool RunCompiler;

    static String FirstStartupKey
    {
        get { return "$Bolt$First$Startup$Wizard/" + BoltNetwork.Version; }
    }

    static Vector2 WindowSize;
    static Vector2 WindowPosition;

    static string AppIdOrEmail = "";

    [NonSerialized] Func<bool> beforeNextCallback;

    [NonSerialized] Dictionary<BoltInstalls, BoltPackage> packageInfo;

    [NonSerialized] int ButtonWidth;

    [NonSerialized] int NavMenuWidth;

    [NonSerialized] string ReleaseHistoryHeader;
    [NonSerialized] List<string> ReleaseHistoryTextAdded;
    [NonSerialized] List<string> ReleaseHistoryTextChanged;
    [NonSerialized] List<string> ReleaseHistoryTextFixed;
    [NonSerialized] List<string> ReleaseHistoryTextRemoved;

    // GUI

    [NonSerialized] Texture2D introIcon;

    [NonSerialized] Texture2D releaseIcon;

    [NonSerialized] Texture2D photonCloudIcon;

    [NonSerialized] Texture2D boltIcon;

    [NonSerialized] Texture2D activeIcon;

    [NonSerialized] Texture2D inactiveIcon;

    [NonSerialized] Texture2D bugtrackerIcon;

    [NonSerialized] GUIContent bugtrackerHeader;

    [NonSerialized] GUIContent bugtrackerText;

    [NonSerialized] Texture2D discordIcon;

    [NonSerialized] GUIContent discordHeader;

    [NonSerialized] GUIContent discordText;

    [NonSerialized] Texture2D documentationIcon;

    [NonSerialized] GUIContent documentationHeader;

    [NonSerialized] GUIContent documentationText;

    [NonSerialized] Texture2D reviewIcon;

    [NonSerialized] GUIContent reviewHeader;

    [NonSerialized] GUIContent reviewText;

    [NonSerialized] Texture2D samplesIcon;

    [NonSerialized] GUIStyle iconSection;

    [NonSerialized] GUIStyle stepStyle;

    [NonSerialized] GUIStyle headerStyle;

    [NonSerialized] GUIStyle headerLabel;

    [NonSerialized] GUIStyle headerLargeLabel;

    [NonSerialized] GUIStyle textLabel;

    [NonSerialized] GUIStyle centerInputText;

    [NonSerialized] GUIStyle minimalButton;

    [NonSerialized] GUIStyle simpleButton;

    [NonSerialized] GUIStyle introStyle;

    [NonSerialized] Vector2 scrollPosition;

    static BoltWizardWindow()
    {
        EditorApplication.update -= ShowWizardWindow;
        EditorApplication.update += ShowWizardWindow;

        WindowPosition = new Vector2(100, 100);
    }

    static void ShowWizardWindow()
    {
        if (FirstCall.HasValue == false)
        {
            FirstCall = Time.realtimeSinceStartup;
            return;
        }

        if ((Time.realtimeSinceStartup - FirstCall.Value) > 1)
        {
            if (!EditorPrefs.GetBool(FirstStartupKey, false))
            {
                Open();
            }

            EditorApplication.update -= ShowWizardWindow;
        }
    }

    [MenuItem("Bolt/Wizard", priority = 100)]
    public static void Open()
    {
        if (Application.isPlaying)
        {
            return;
        }

        BoltWizardWindow window = GetWindow<BoltWizardWindow>(true, BoltWizardText.WINDOW_TITLE, true);
        window.position = new Rect(WindowPosition, WindowSize);
        window.Show();

        watch.Start();
    }

    static void ReOpen()
    {
        if (Ready.HasValue && Ready.Value == false)
        {
            Open();
        }

        EditorApplication.update -= ReOpen;
    }

    void OnEnable()
    {
        WindowSize = new Vector2(600, 600);

        minSize = WindowSize;

        NavMenuWidth = 210;
        ButtonWidth = 120;

        Ready = false;

        beforeNextCallback = null;

        // Pre-load Release History
        PrepareReleaseHistoryText();
    }

    void OnDestroy()
    {
        if (!EditorPrefs.GetBool(FirstStartupKey, false))
        {
            if (!EditorUtility.DisplayDialog(BoltWizardText.CLOSE_MSG_TITLE,
                BoltWizardText.CLOSE_MSG_QUESTION, "Yes", "Back"))
            {
                EditorApplication.update += ReOpen;
            }
        }

        Ready = false;
    }

    void InitContent()
    {
        if (Ready.HasValue && Ready.Value)
        {
            return;
        }

        introIcon = Resources.Load<Texture2D>("icons_welcome/information");
        releaseIcon = Resources.Load<Texture2D>("icons_welcome/documentation");
        photonCloudIcon = Resources.Load<Texture2D>("PhotonCloudIcon");

        boltIcon = Resources.Load<Texture2D>("BoltIcon");

        activeIcon = Resources.Load<Texture2D>("icons_welcome/bullet_green");
        inactiveIcon = Resources.Load<Texture2D>("icons_welcome/bullet_black");

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.gray);
        texture.Apply();

        Color headerTextColor = EditorGUIUtility.isProSkin
            ? new Color(0xf2 / 255f, 0xad / 255f, 0f)
            : new Color(30 / 255f, 99 / 255f, 183 / 255f);
        Color commonTextColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        introStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 15,
            padding = new RectOffset(10, 10, 10, 10),
            normal =
            {
                textColor = commonTextColor
            }
        };

        stepStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 5, 0),
            normal =
            {
                textColor = commonTextColor
            }
        };

        headerLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            padding = new RectOffset(10, 0, 0, 0),
            margin = new RectOffset(),
            normal =
            {
                textColor = commonTextColor
            }
        };

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            margin = new RectOffset(),
            padding = new RectOffset(10, 0, 0, 0),
            normal =
            {
                textColor = commonTextColor
            }
        };

        headerLargeLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            padding = new RectOffset(10, 0, 0, 0),
            margin = new RectOffset(),
            fontSize = 18,
            normal =
            {
                textColor = headerTextColor
            }
        };

        textLabel = new GUIStyle()
        {
            wordWrap = true,
            margin = new RectOffset(),
            padding = new RectOffset(10, 0, 0, 0),
            normal =
            {
                textColor = commonTextColor
            }
        };

        centerInputText = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fixedHeight = 26
        };

        minimalButton = new GUIStyle(EditorStyles.miniButton)
        {
            fixedWidth = 130
        };

        simpleButton = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            padding = new RectOffset(10, 10, 10, 10)
        };

        iconSection = new GUIStyle
        {
            margin = new RectOffset(0, 0, 0, 0)
        };

        discordIcon = Resources.Load<Texture2D>("icons_welcome/community");
        discordText = new GUIContent(BoltWizardText.DISCORD_TEXT);
        discordHeader = new GUIContent(BoltWizardText.DISCORD_HEADER);

        bugtrackerIcon = Resources.Load<Texture2D>("icons_welcome/bugtracker");
        bugtrackerText = new GUIContent(BoltWizardText.BUGTRACKER_TEXT);
        bugtrackerHeader = new GUIContent(BoltWizardText.BUGTRACKER_HEADER);

        documentationIcon = Resources.Load<Texture2D>("icons_welcome/documentation");
        documentationText = new GUIContent(BoltWizardText.DOCUMENTATION_TEXT);
        documentationHeader = new GUIContent(BoltWizardText.DOCUMENTATION_HEADER);

        reviewIcon = Resources.Load<Texture2D>("icons_welcome/comments");
        reviewText = new GUIContent(BoltWizardText.REVIEW_TEXT);
        reviewHeader = new GUIContent(BoltWizardText.REVIEW_HEADER);

        samplesIcon = Resources.Load<Texture2D>("icons_welcome/samples");

        // Package List

        packageInfo = new Dictionary<BoltInstalls, BoltPackage>
        {
            {
                BoltInstalls.Core,
                new BoltPackage()
                {
                    name = "bolt_install",
                    title = "Core Package",
                    description = "Install core bolt package",
                    installTest = MainPackageInstalled,
                    packageFlags = PackageFlags.RunInitialSetup
                }
            },

            // {
            // 	BoltInstalls.Mobile,
            // 	new BoltPackage()
            // 	{
            // 		name = "bolt_mobile_plugins",
            // 		title = "Mobile Plugins",
            // 		installTest = MobilePackageInstalled,
            // 		description = "Install iOS / Android socket plugins"
            // 	}
            // },

            {
                BoltInstalls.XB1,
                new BoltPackage()
                {
                    name = "bolt_xb1",
                    title = "XBox One",
                    installTest = XB1PackageInstalled,
                    description = "Install XB1 support"
                }
            },

            {
                BoltInstalls.PS4,
                new BoltPackage()
                {
                    name = "bolt_ps4",
                    title = "Playstation 4",
                    installTest = PS4PackageInstalled,
                    description = "Install PS4 support"
                }
            },

            {
                BoltInstalls.Samples,
                new BoltPackage()
                {
                    name = "bolt_samples",
                    title = "Samples",
                    description = "Install bolt samples",
                    installTest = SamplesPackageInstalled,
                    packageFlags = PackageFlags.WarnForProjectOverwrite
                }
            }
        };

        // App ID
        if (string.IsNullOrEmpty(BoltRuntimeSettings.instance.photonAppId))
        {
            AppIdOrEmail = "";
        }
        else if (IsAppId(BoltRuntimeSettings.instance.photonAppId))
        {
            AppIdOrEmail = BoltRuntimeSettings.instance.photonAppId;
        }

        Ready = true;
        RunCompiler = false;
    }

    void OnGUI()
    {
        try
        {
            InitContent();

            WindowPosition = position.position;

            EditorGUILayout.BeginVertical();
            DrawHeader();

            // Content
            EditorGUILayout.BeginHorizontal();

            // Nav menu
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(NavMenuWidth),
                GUILayout.MinWidth(NavMenuWidth));
            DrawNavMenu();
            EditorGUILayout.EndVertical();

            // Main Content
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawContent();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                Save();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void DrawContent()
    {
        switch (currentStage)
        {
            case BoltSetupStage.Intro:
                DrawIntro();
                break;
            case BoltSetupStage.ReleaseHistory:
                DrawReleaseHistory();
                break;
            case BoltSetupStage.Photon:
                DrawSetupPhoton();
                break;
            case BoltSetupStage.Bolt:
                DrawSetupBolt();
                break;
            case BoltSetupStage.Support:
                DrawSupport();
                break;
        }

        GUILayout.FlexibleSpace();
        DrawFooter();
    }

    private void DrawIntro()
    {
        GUILayout.BeginVertical();
        GUILayout.Label(BoltWizardText.WIZARD_INTRO, introStyle);

        if (GUILayout.Button("Visit Getting Started Documentation", simpleButton))
        {
            OpenURL("https://doc.photonengine.com/en-us/bolt/")();
        }

        if (GUILayout.Button("Leave a review", simpleButton))
        {
            OpenURL("https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156")();
        }

        GUILayout.EndVertical();
    }

    private void DrawSetupBolt()
    {
        DrawInputWithLabel("Bolt Setup", () =>
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label(BoltWizardText.PACKAGES, textLabel);
            GUILayout.EndVertical();
        }, false);
        GUILayout.Space(15);

        DrawInstallOption(BoltInstalls.Core);

        EditorGUI.BeginDisabledGroup(!IsInstalled(BoltInstalls.Core));

        // SAMPLES
        DrawInstallOption(BoltInstalls.Samples);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // // MOBILE
        // DrawInstallOption(BoltInstalls.Mobile);

        // XB1
        DrawInstallOption(BoltInstalls.XB1);

        // PS4
        DrawInstallOption(BoltInstalls.PS4);

        EditorGUILayout.EndScrollView();

        EditorGUI.EndDisabledGroup();

        // Action

        if (beforeNextCallback == null)
        {
            beforeNextCallback = () =>
            {
                if (!IsInstalled(BoltInstalls.Core))
                {
                    ShowNotification(new GUIContent("You must install at least the Bolt Core package."));
                    return false;
                }

                return true;
            };
        }
    }

    private void DrawReleaseHistory()
    {
        DrawInputWithLabel(string.Format("Version Changelog: {0}", ReleaseHistoryHeader), () =>
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));

            DrawReleaseHistoryItem("Added:", ReleaseHistoryTextAdded);
            DrawReleaseHistoryItem("Changed:", ReleaseHistoryTextChanged);
            DrawReleaseHistoryItem("Fixed:", ReleaseHistoryTextFixed);
            DrawReleaseHistoryItem("Removed:", ReleaseHistoryTextRemoved);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }, false, labelSize: 300);
    }

    private void DrawReleaseHistoryItem(string label, List<string> items)
    {
        if (items != null && items.Count > 0)
        {
            DrawInputWithLabel(label, () =>
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                
                foreach (var text in items)
                {
                    GUILayout.Label(string.Format("- {0}.", text), textLabel);
                }

                GUILayout.EndVertical();
            }, false, true, 200);
        }
    }

    private void DrawSetupPhoton()
    {
        DrawInputWithLabel("Photon Cloud Setup", () =>
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label(BoltWizardText.PHOTON, textLabel);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(BoltWizardText.PHOTON_DASH, textLabel);
            if (GUILayout.Button("Visit Dashboard", minimalButton))
            {
                OpenURL("https://dashboard.photonengine.com/")();
            }

            GUILayout.EndHorizontal();
        }, false);
        GUILayout.Space(15);

        BoltRuntimeSettings settings = BoltRuntimeSettings.instance;

        DrawInputWithLabel("Photon Bolt App ID or Email", () =>
        {
            GUILayout.BeginVertical();

            AppIdOrEmail = EditorGUILayout.TextField(AppIdOrEmail, centerInputText);

            GUILayout.EndVertical();
        }, false, true, 300);

        DrawInputWithLabel("Region",
            () =>
            {
                settings.photonCloudRegionIndex = EditorGUILayout.Popup(settings.photonCloudRegionIndex,
                    BoltRuntimeSettings.photonCloudRegions);
            }, true, true);

        DrawInputWithLabel("NAT Punchthrough Enabled",
            () => { settings.photonUsePunch = BoltEditorGUI.Toggle(settings.photonUsePunch); }, true, true);

        // Action

        if (beforeNextCallback == null)
        {
            beforeNextCallback = () =>
            {
                if (requestingAppId)
                {
                    BoltLog.Info("Please, wait until your request for a new AppID finishes.");
                    return false;
                }

                if (AccountService.IsValidEmail(AppIdOrEmail))
                {
                    try
                    {
						EditorUtility.DisplayProgressBar(BoltWizardText.CONNECTION_TITLE,
                            BoltWizardText.CONNECTION_INFO, 0.5f);
                        BoltLog.Info("Starting request");

                        requestingAppId = new AccountService().RegisterByEmail(
                            AppIdOrEmail,
                            new List<ServiceTypes>() {ServiceTypes.Bolt},
                            (response) =>
                            {
                                if (response.ReturnCode == AccountServiceReturnCodes.Success)
                                {
                                    var appKey = response.ApplicationIds[((int) ServiceTypes.Bolt).ToString()];

                                    settings.photonAppId = appKey;
                                    AppIdOrEmail = appKey;

                                    BoltLog.Info("You new App ID: {0}", AppIdOrEmail);
                                }
                                else
                                {
                                    BoltLog.Warn(
                                        "It was not possible to process your request, please go to the Photon Cloud Dashboard.");
                                    BoltLog.Warn("Return Code: {0}",
                                        AccountServiceReturnCodes.ReturnCodes[response.ReturnCode]);
                                }

                                requestingAppId = false;
								EditorUtility.ClearProgressBar();
                            }, (err) =>
                            {
                                BoltLog.Error(err);

                                requestingAppId = false;
								EditorUtility.ClearProgressBar();
                            });

                        if (requestingAppId)
                        {
                            BoltLog.Info("Requesting your new App ID");
                        }
                        else
                        {
                            BoltLog.Warn(
                                "It was not possible to process your request, please go to the Photon Cloud Dashboard.");
							EditorUtility.ClearProgressBar();
                        }
                    }
                    catch (Exception ex)
                    {
						EditorUtility.DisplayDialog("Error", ex.Message, "ok");
                    }
                }
                else if (IsAppId(AppIdOrEmail))
                {
                    settings.photonAppId = AppIdOrEmail;
                    return true;
                }
                else
                {
                    ShowNotification(new GUIContent("Please specify a valid Photon Bolt App ID or Email."));
                }

                return false;
            };
        }
    }

    private void DrawSupport()
    {
        DrawInputWithLabel("Bolt Support", () =>
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label(BoltWizardText.SUPPORT, textLabel);
            GUILayout.EndVertical();
        }, false);
        GUILayout.Space(15);

        DrawStepOption(discordIcon, discordHeader, discordText,
            callback: OpenURL("https://discord.gg/0ya6ZpOvnShSCtbb"));
        DrawStepOption(bugtrackerIcon, bugtrackerHeader, bugtrackerText,
            callback: OpenURL("https://github.com/BoltEngine/Bolt-Tracker"));
        DrawStepOption(documentationIcon, documentationHeader, documentationText,
            callback: OpenURL("https://doc.photonengine.com/en-us/bolt/current/setup/overview"));
        DrawStepOption(reviewIcon, reviewHeader, reviewText,
            callback: OpenURL("https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156"));

        // Action

        if (beforeNextCallback == null)
        {
            beforeNextCallback = () =>
            {
                RunCompiler = EditorUtility.DisplayDialog(BoltWizardText.FINISH_TITLE, BoltWizardText.FINISH_QUESTION,
                    "Yes", "No");
                return true;
            };
        }
    }

    private void DrawNavMenu()
    {
        GUILayout.Space(5);
        DrawMenuHeader("Installation Steps");
        GUILayout.Space(10);

        DrawStepOption(introIcon, new GUIContent("Bolt Wizard Intro"),
            active: currentStage ==
                    BoltSetupStage.Intro);

        DrawStepOption(releaseIcon, new GUIContent("Release History"),
            active: currentStage ==
                    BoltSetupStage.ReleaseHistory);

        DrawStepOption(photonCloudIcon, new GUIContent("Photon Cloud"),
            active: currentStage ==
                    BoltSetupStage.Photon);

        DrawStepOption(boltIcon, new GUIContent("Bolt"),
            active: currentStage ==
                    BoltSetupStage.Bolt);

        DrawStepOption(discordIcon, new GUIContent("Support"),
            active: currentStage ==
                    BoltSetupStage.Support);

        GUILayout.FlexibleSpace();
        GUILayout.Label(BoltNetwork.CurrentVersion, textLabel);
        GUILayout.Space(5);
    }

    void DrawHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(Resources.Load<Texture2D>("BoltLogo"), GUILayout.Width(200), GUILayout.Height(109));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DrawFooter()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginDisabledGroup((int) currentStage == 1);

#if !BOLT_CLOUD
		if (currentStage == BoltSetupStage.Photon)
		{
			if (GUILayout.Button("Skip", GUILayout.Width(ButtonWidth)))
			{
				NextStep();
				beforeNextCallback = null;
			}
		}
#endif

        if (GUILayout.Button("Back", GUILayout.Width(ButtonWidth)))
        {
            beforeNextCallback = null;
            BackStep();
        }

        EditorGUI.EndDisabledGroup();

        var nextLabel = "Next";

        switch (currentStage)
        {
            case BoltSetupStage.Photon:
                nextLabel = AccountService.IsValidEmail(AppIdOrEmail) ? "Register by Email" : nextLabel;
                break;
            case BoltSetupStage.Support:
                nextLabel = "Done";
                break;
            default:
                nextLabel = "Next";
                break;
        }

        if (GUILayout.Button(nextLabel, GUILayout.Width(ButtonWidth)))
        {
            if (beforeNextCallback == null || beforeNextCallback())
            {
                if (currentStage == BoltSetupStage.Support)
                {
                    EditorPrefs.SetBool(FirstStartupKey, true);
                    Close();

                    if (RunCompiler)
                    {
                        BoltMenuItems.RunCompiler();
                    }
                }

                NextStep();
                beforeNextCallback = null;
            }
        }

        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    // Utils

    private void Save()
    {
        if (watch.ElapsedMilliseconds > 5000)
        {
            watch.Reset();
            watch.Start();

            EditorUtility.SetDirty(BoltRuntimeSettings.instance);
            AssetDatabase.SaveAssets();
        }
    }

    void DrawMenuHeader(String text)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label(text, headerLargeLabel);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DrawInputWithLabel(String label, Action gui, bool horizontal = true, bool box = false, int labelSize = 220)
    {
        GUILayout.Space(10);

        if (horizontal)
        {
            if (box)
            {
                GUILayout.BeginHorizontal(stepStyle);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }
        }
        else
        {
            if (box)
            {
                GUILayout.BeginVertical(stepStyle);
            }
            else
            {
                GUILayout.BeginVertical();
            }
        }

        GUILayout.Label(label, headerStyle, GUILayout.Width(labelSize));
        //GUILayout.Space(5);

        gui();

        GUILayout.Space(5);

        if (horizontal)
        {
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.EndVertical();
        }
    }

    void DrawInstallOption(BoltInstalls install)
    {
        BoltPackage package = packageInfo[install];

        Action action = () =>
        {
            if (package.installTest())
            {
                ShowNotification(new GUIContent("Package already installed"));
                return;
            }

            Install(package);
        };

        bool packageExists = PackageExists(package.name);

        Action ignoredAction;
        if (packageExists == true)
        {
            ignoredAction = () => { ShowNotification(new GUIContent("One of the dependencies is missing")); };
        }
        else
        {
            if (install == BoltInstalls.Core)
            {
                ignoredAction = () =>
                {
                    ShowNotification(new GUIContent("Bolt Core is no longer required. You are ready to go."));
                };
            }
            else
            {
                ignoredAction = () =>
                {
                    ShowNotification(new GUIContent("Please contact us at developer@photonengine.com"));
                };
            }

            EditorGUI.BeginDisabledGroup(true);
        }

        DrawStepOption(samplesIcon, new GUIContent(package.title), new GUIContent(package.description),
            package.installTest(), action, ignoredAction);

        if (packageExists == false)
        {
            EditorGUI.EndDisabledGroup();
        }
    }

    void DrawStepOption(Texture2D icon, GUIContent header, GUIContent description = null, bool? active = null,
        Action callback = null, Action ignoredCallback = null)
    {
        GUILayout.BeginHorizontal(stepStyle);

        if (icon != null)
        {
            GUILayout.Label(icon, iconSection, GUILayout.Width(32), GUILayout.Height(32));
        }

        var height = icon != null ? 32 : 16;

        GUILayout.BeginVertical(GUILayout.MinHeight(height));
        GUILayout.FlexibleSpace();

        GUILayout.Label(header, headerLabel, GUILayout.MinWidth(120));

        if (description != null)
        {
            GUILayout.Label(description, textLabel);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        if (active == true)
        {
            GUILayout.Label(activeIcon, iconSection, GUILayout.Width(height), GUILayout.Height(height));
        }
        else if (active == false)
        {
            GUILayout.Label(inactiveIcon, iconSection, GUILayout.Width(height), GUILayout.Height(height));
        }

        GUILayout.EndHorizontal();

        if (callback != null)
        {
            var rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    callback();
                    GUIUtility.ExitGUI();
                }
                else if (Event.current.type == EventType.Ignore && Event.current.rawType == EventType.MouseDown)
                {
                    if (ignoredCallback != null)
                    {
                        ignoredCallback();
                    }
                }
            }
        }
    }

    void NextStep()
    {
        currentStage += (int) currentStage < Enum.GetValues(typeof(BoltSetupStage)).Length ? 1 : 0;
    }

    void BackStep()
    {
        currentStage -= (int) currentStage > 1 ? 1 : 0;
    }

    bool IsInstalled(params BoltInstalls[] installs)
    {
        foreach (var pack in installs)
        {
            if (!packageInfo[pack].installTest())
            {
                return false;
            }
        }

        return true;
    }

    void Install(BoltPackage package)
    {
        string packageName = package.name;
        PackageFlags flags = package.packageFlags;

        if ((flags & PackageFlags.WarnForProjectOverwrite) == PackageFlags.WarnForProjectOverwrite)
        {
            if (ProjectExists())
            {
                if (EditorUtility.DisplayDialog("Warning",
                        "Importing this package will overwrite the existing bolt project file that contains all your states, events, etc. Are you sure?",
                        "Yes", "No") == false)
                {
                    return;
                }
            }
        }

        if ((flags & PackageFlags.RunInitialSetup) == PackageFlags.RunInitialSetup)
        {
            InitialSetup();
        }

        AssetDatabase.ImportPackage(PackagePath(packageName), false);

        currentStage = BoltSetupStage.Bolt;
    }
}