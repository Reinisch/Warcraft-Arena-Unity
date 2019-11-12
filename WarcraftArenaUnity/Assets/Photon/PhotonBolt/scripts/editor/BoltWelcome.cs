using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Bolt.Editor;

// [InitializeOnLoad]
public class BoltWelcome : EditorWindow
{
	[Flags]
	protected enum PackageFlags
	{
		WarnForProjectOverwrite = 1 << 1,
		RunInitialSetup = 1 << 2
	}

	static String FirstStartupKey
	{
		get { return "$Bolt$First$Startup/" + BoltNetwork.CurrentVersion; }
	}

	static String ShowAtStartupKey
	{
		get { return "$Bolt$Show$At$Startup"; }
	}

	const Boolean SHOW_AT_STARTUP_DEFAULT = true;

	static Single? _firstCall;

	static BoltWelcome()
	{
		// EditorApplication.update -= ShowWelcomeMessage;
		// EditorApplication.update += ShowWelcomeMessage;
	}

	static void ShowWelcomeMessage()
	{
		var show = true;

#if !BOLT_DEV_INSTALLER
		if (EditorApplication.timeSinceStartup > 32f)
		{
			if (EditorPrefs.GetBool(FirstStartupKey, false))
			{
				show = false;
			}
		}
#endif

		// if disabled
		if (EditorPrefs.GetBool(ShowAtStartupKey, SHOW_AT_STARTUP_DEFAULT) == false)
		{
#if !BOLT_DEV_INSTALLER
			show = false;
#endif
		}

		if (show)
		{
			if (_firstCall.HasValue == false)
			{
				_firstCall = Time.realtimeSinceStartup;
			}

			if ((Time.realtimeSinceStartup - _firstCall.Value) > 1)
			{
				// remove callback right away
				EditorApplication.update -= ShowWelcomeMessage;

				// open window
				Open();
			}
		}
		else
		{
			EditorApplication.update -= ShowWelcomeMessage;
		}
	}

	// [MenuItem("Window/Bolt/Welcome")]
	static void Open()
	{
		// set first startup flag
		EditorPrefs.SetBool(FirstStartupKey, true);

		BoltWelcome w;
		w = GetWindow<BoltWelcome>(true);
		w.titleContent = new GUIContent("Welcome To Photon Bolt");
	}

	[NonSerialized]
	protected Boolean? _init;

	[NonSerialized]
	protected GUIStyle _iconSection;

	[NonSerialized]
	protected GUIStyle _headerLabel;

	[NonSerialized]
	GUIStyle _headerLargeLabel;

	[NonSerialized]
	protected GUIStyle _textSection;

	[NonSerialized]
	protected GUIStyle _textLabel;

	[NonSerialized]
	protected Texture2D _bugtrackerIcon;

	[NonSerialized]
	protected GUIContent _bugtrackerHeader;

	[NonSerialized]
	protected GUIContent _bugtrackerText;

	[NonSerialized]
	protected Texture2D _discordIcon;

	[NonSerialized]
	protected GUIContent _discordHeader;

	[NonSerialized]
	protected GUIContent _discordText;

	[NonSerialized]
	protected Texture2D _documentationIcon;

	[NonSerialized]
	protected GUIContent _documentationHeader;

	[NonSerialized]
	protected GUIContent _documentationText;

	[NonSerialized]
	protected Texture2D _samplesIcon;

#pragma warning disable
	[NonSerialized]
	protected GUIContent _samplesHeader;

	[NonSerialized]
	protected GUIContent _samplesText;
#pragma warning restore

	Vector2 scrollController;

	void OnGUI()
	{
		this.position = new Rect(this.position.x, this.position.y, 380, 900);

		scrollController = GUILayout.BeginScrollView(scrollController);

		InitContent();

		GUILayout.BeginVertical();

		MainMenu();

		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();

		GUILayout.Label(BoltNetwork.CurrentVersion, _textLabel);

		GUILayout.FlexibleSpace();
		EditorPrefs.SetBool(ShowAtStartupKey, GUILayout.Toggle(EditorPrefs.GetBool(ShowAtStartupKey, SHOW_AT_STARTUP_DEFAULT), "Always Show This On Startup"));

		GUILayout.EndHorizontal();

		GUILayout.Space(8);
		GUILayout.EndVertical();

		GUILayout.EndScrollView();
	}

	void MainMenu()
	{
		GUILayout.Space(32);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(Resources.Load<Texture2D>("BoltLogo"), GUILayout.Width(256), GUILayout.Height(139));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(32);

		DrawMenuHeader("Packages");
		DrawMenuInstall("bolt_install", "Core Package", "Install core bolt package", PackageFlags.RunInitialSetup);

		EditorGUI.BeginDisabledGroup(MainPackageInstalled() == false);

		// MOBILE
		DrawMenuInstall("bolt_mobile_plugins", "Mobile Plugins", "Install iOS/Android socket plugins");

		// SAMPLES
		DrawMenuInstall("bolt_samples", "Samples", "Install bolt samples", PackageFlags.WarnForProjectOverwrite);

		// XBox One
		DrawMenuInstall("bolt_xb1", "XBox One", "Install XB1 support");

		// PS4
		DrawMenuInstall("bolt_ps4", "Playstation 4", "Install PS4 support");

		EditorGUI.EndDisabledGroup();

		GUILayout.Space(16);

		DrawMenuHeader("Support");
		DrawMenuOption(_documentationIcon, _documentationHeader, _documentationText, OpenURL("https://doc.photonengine.com/en-us/bolt/current/setup/overview"));
		DrawMenuOption(_discordIcon, _discordHeader, _discordText, OpenURL("https://discord.gg/0ya6ZpOvnShSCtbb"));
		DrawMenuOption(_bugtrackerIcon, _bugtrackerHeader, _bugtrackerText, OpenURL("https://github.com/BoltEngine/Bolt-Tracker"));
	}

	virtual protected void InitContent()
	{
		if (_init.HasValue && _init.Value)
		{
			return;
		}

		_init = true;

		_textLabel = new GUIStyle("Label");
		_textLabel.wordWrap = true;
		_textLabel.margin = new RectOffset();
		_textLabel.padding = new RectOffset(10, 0, 0, 0);

		_textSection = new GUIStyle();
		_iconSection = new GUIStyle();
		_iconSection.margin = new RectOffset(0, 0, 0, 0);

		_headerLabel = new GUIStyle(EditorStyles.boldLabel);
		_headerLabel.padding = new RectOffset(10, 0, 0, 0);
		_headerLabel.margin = new RectOffset();

		_headerLargeLabel = new GUIStyle(EditorStyles.boldLabel);
		_headerLargeLabel.padding = new RectOffset(10, 0, 0, 0);
		_headerLargeLabel.margin = new RectOffset();
		_headerLargeLabel.fontSize = 18;
		_headerLargeLabel.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0xf2 / 255f, 0xad / 255f, 0f) : new Color(30 / 255f, 99 / 255f, 183 / 255f);

		_discordIcon = Resources.Load<Texture2D>("icons_welcome/community");
		_discordText = new GUIContent("Join the Bolt Discord Community.");
		_discordHeader = new GUIContent("Community");

		_bugtrackerIcon = Resources.Load<Texture2D>("icons_welcome/bugtracker");
		_bugtrackerText = new GUIContent("Open bugtracker on github.");
		_bugtrackerHeader = new GUIContent("Bug Tracker");

		_documentationIcon = Resources.Load<Texture2D>("icons_welcome/documentation");
		_documentationText = new GUIContent("Open the documentation.");
		_documentationHeader = new GUIContent("Documentation");

		_samplesIcon = Resources.Load<Texture2D>("icons_welcome/samples");
		_samplesText = new GUIContent("Import the samples package.");
		_samplesHeader = new GUIContent("Samples");
	}

	protected Action OpenURL(String url, params System.Object[] args)
	{
		return () =>
		{
			if (args.Length > 0)
			{
				url = String.Format(url, args);
			}

			Application.OpenURL(url);
		};
	}

	protected void DrawMenuHeader(String text)
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		GUILayout.Label(text, _headerLargeLabel);

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	protected void DrawMenuInstall(String packageName, String title, String description, PackageFlags packageFlags = default(PackageFlags))
	{
		bool packageExists = PackageExists(packageName);

		Action ignoredAction;
		if (packageExists == true)
		{
			ignoredAction = () => { ShowNotification(new GUIContent("One of the dependencies is missing")); };
		}
		else
		{
			ignoredAction = () => { ShowNotification(new GUIContent("Please contact us at developer@photonengine.com")); };

			EditorGUI.BeginDisabledGroup(true);
		}

		DrawMenuOption(_samplesIcon, new GUIContent(title), new GUIContent(description), InstallPackage(packageName, packageFlags), ignoredAction);

		if (packageExists == false)
		{
			EditorGUI.EndDisabledGroup();
		}
	}

	protected void DrawMenuOption(Texture2D icon, GUIContent header, GUIContent text, System.Action callback = null, Action ignoredCallback = null)
	{
		GUILayout.Space(16);
		GUILayout.BeginHorizontal();

		GUILayout.Space(80);
		GUILayout.Label(icon, _iconSection, GUILayout.Width(32), GUILayout.Height(32));

		GUILayout.BeginVertical(_textSection);
		GUILayout.Label(header, _headerLabel);
		GUILayout.Label(text, _textLabel);
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

		if (rect.Contains(Event.current.mousePosition))
		{
			if (Event.current.type == EventType.MouseDown)
			{
				if (callback != null)
				{
					callback();
				}

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

	protected String PackagePath(String packageName)
	{
		return Path.Combine(BoltPathUtility.PackagesPath, packageName + ".unitypackage");
	}

	protected Boolean PackageExists(String packageName)
	{
		return File.Exists(PackagePath(packageName));
	}

	protected Boolean ProjectExists()
	{
		return File.Exists(BoltPathUtility.ProjectPath);
	}

	protected Boolean MainPackageInstalled()
	{
		return File.Exists(Path.Combine(BoltPathUtility.ScriptsPath, "BoltLauncher.cs"));
	}

	protected Boolean SamplesPackageInstalled()
	{
		return Directory.Exists("Assets/samples");
	}

	protected Boolean SteamPackageInstalled()
	{
		return File.Exists("Assets/Plugins/x86/CSteamworks.dll");
	}

	protected Boolean MobilePackageInstalled()
	{
		return Directory.Exists("Assets/Plugins/iOS") && Directory.Exists("Assets/Plugins/Android");
	}

	protected Action InstallPackage(String packageName, PackageFlags packageFlags)
	{
		return () =>
		{
			if ((packageFlags & PackageFlags.WarnForProjectOverwrite) == PackageFlags.WarnForProjectOverwrite)
			{
				if (ProjectExists())
				{
					if (EditorUtility.DisplayDialog("Warning", "Importing this package will overwrite the existing bolt project file that contains all your states, events, etc. Are you sure?", "Yes", "No") == false)
					{
						return;
					}
				}
			}

			if ((packageFlags & PackageFlags.RunInitialSetup) == PackageFlags.RunInitialSetup)
			{
				InitialSetup();
			}

			AssetDatabase.ImportPackage(PackagePath(packageName), false);
		};
	}

	protected void InitialSetup()
	{
		string SETTINGS_PATH = Path.Combine(BoltPathUtility.ResourcesPath, "BoltRuntimeSettings.asset");
		string PREFABDB_PATH = Path.Combine(BoltPathUtility.ResourcesPath, "BoltPrefabDatabase.asset");

		if (!AssetDatabase.LoadAssetAtPath(SETTINGS_PATH, typeof(BoltRuntimeSettings)))
		{
			BoltRuntimeSettings settings = CreateInstance<BoltRuntimeSettings>();
			settings.masterServerGameId = Guid.NewGuid().ToString().ToUpperInvariant();

			AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
			AssetDatabase.ImportAsset(SETTINGS_PATH, ImportAssetOptions.Default);
		}

		if (!AssetDatabase.LoadAssetAtPath(PREFABDB_PATH, typeof(Bolt.PrefabDatabase)))
		{
			AssetDatabase.CreateAsset(CreateInstance<Bolt.PrefabDatabase>(), PREFABDB_PATH);
			AssetDatabase.ImportAsset(PREFABDB_PATH, ImportAssetOptions.Default);
		}

		BoltMenuItems.RunCompiler();
	}
}
