using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Bolt.Editor;
using Bolt.Editor.Utils;

public partial class BoltWizardWindow
{
	[Flags]
	enum PackageFlags
	{
		WarnForProjectOverwrite = 1 << 1,
		RunInitialSetup = 1 << 2
	}

	class BoltWizardText
	{
		internal static readonly string WINDOW_TITLE = "Bolt Wizard";
		internal static readonly string SUPPORT = "You can contact the Bolt Team or Photon Services using one of the following links. You can also go to Bolt Documentation in order to get started with Photon Bolt.";
		internal static readonly string PACKAGES = "Here you will be able to select all packages you want to use into your project. Packages marked green are already installed. \nClick to install.";
		internal static readonly string PHOTON = "In this step, you will configure your Photon Cloud credentials in order to use our servers for matchmaking, relay and much more. Please fill all fields with your desired configuration.";
		internal static readonly string PHOTON_DASH = "Go to Dashboard to create your App ID: ";

		internal static readonly string CONNECTION_TITLE = "Connecting";
		internal static readonly string CONNECTION_INFO = "Connecting to the account service...";

		internal static readonly string FINISH_TITLE = "Bolt Setup Complete";
		internal static readonly string FINISH_QUESTION = "In order to finish the setup, Bolt needs to compile all Assets, do you want to proceed? If you opt to not compile now, you can run it on \"Bolt/Compile Assembly\".";

		internal static readonly string CLOSE_MSG_TITLE = "Incomplete Installation";
		internal static readonly string CLOSE_MSG_QUESTION = "Are you sure you want to exit the Wizard?";
		internal static readonly string DISCORD_TEXT = "Join the Bolt Discord Community.";
		internal static readonly string DISCORD_HEADER = "Community";
		internal static readonly string BUGTRACKER_TEXT = "Open bugtracker on github.";
		internal static readonly string BUGTRACKER_HEADER = "Bug Tracker";
		internal static readonly string DOCUMENTATION_TEXT = "Open the documentation.";
		internal static readonly string DOCUMENTATION_HEADER = "Documentation";
		internal static readonly string REVIEW_TEXT = "Please, let others know what you think about Bolt.";
		internal static readonly string REVIEW_HEADER = "Leave a review";
		internal static readonly string SAMPLES_TEXT = "Import the samples package.";
		internal static readonly string SAMPLES_HEADER = "Samples";
		internal static readonly string WIZARD_INTRO =
@"Hello! Welcome to Bolt Wizard!

We are glad that you decided to use our products and services. Here at Photon, we work hard to make your multiplayer game easier to build and much more fun to play.

This is a simple step by step configuration that you can follow and in just a few minutes you will be ready to use Bolt in all its power. If you have any doubt, please to our Getting Started page.

Please, feel free to click on the Next button, and get started.";
	}

	class BoltPackage
	{
		public string name;
		public string title;
		public string description;
		public Func<bool> installTest;
		public PackageFlags packageFlags = default(PackageFlags);
	}

	enum BoltSetupStage
	{
		Intro = 1,
		ReleaseHistory = 2,
		Photon = 3,
		Bolt = 4,
		Support = 5
	}

	[Flags]
	enum BoltInstalls
	{
		Core = 1 << 0,
		Mobile = 1 << 1,
		Samples = 1 << 3,
		XB1 = 1 << 4,
		PS4 = 1 << 5
	}

	String PackagePath(String packageName)
	{
		return Path.Combine(BoltPathUtility.PackagesPath, packageName + ".unitypackage");
	}

	Boolean PackageExists(String packageName)
	{
		return File.Exists(PackagePath(packageName));
	}

	Boolean ProjectExists()
	{
		return File.Exists(BoltPathUtility.ProjectPath);
	}

	Boolean MainPackageInstalled()
	{
		string SETTINGS_PATH = Path.Combine(BoltPathUtility.ResourcesPath, "BoltRuntimeSettings.asset");
		string PREFABDB_PATH = Path.Combine(BoltPathUtility.ResourcesPath, "BoltPrefabDatabase.asset");

		return File.Exists(SETTINGS_PATH) && File.Exists(PREFABDB_PATH);
	}

	Boolean SamplesPackageInstalled()
	{
		return Directory.Exists("Assets/samples");
	}

	Boolean XB1PackageInstalled()
	{
		return File.Exists("Assets/Plugins/XB1.dll");
	}

	Boolean PS4PackageInstalled()
	{
		return File.Exists("Assets/Plugins/PS4.dll");
	}

	Boolean MobilePackageInstalled()
	{
		return Directory.Exists("Assets/Plugins/iOS") && Directory.Exists("Assets/Plugins/Android");
	}

	Action OpenURL(String url, params System.Object[] args)
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

	void InitialSetup()
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

	static bool IsAppId(string val)
	{
		try
		{
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new Guid(val);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'
		}
		catch
		{
			return false;
		}
		return true;
	}

	private void PrepareReleaseHistoryText()
	{
		var text = (TextAsset)AssetDatabase.LoadAssetAtPath(Path.Combine(BoltPathUtility.BasePath, "release_history.txt"), typeof(TextAsset));
		var baseText = text.text;

		var regexVersion = new Regex(@"(\b(\d+\.)?\d+\.\d+\.\d+\n)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
		var regexAdded = new Regex(@"\b(Added:)(.*)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
		var regexChanged = new Regex(@"\b(Changed:)(.*)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
		var regexFixed = new Regex(@"\b(Fixed:)(.*)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
		var regexRemoved = new Regex(@"\b(Removed:)(.*)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		var matches = regexVersion.Matches(baseText);

		if (matches.Count > 0)
		{
			var currentVersionMatch = matches[0];
			var lastVersionMatch = currentVersionMatch.NextMatch();

			if (currentVersionMatch.Success && lastVersionMatch.Success)
			{
				var header = currentVersionMatch.Value.Trim();
				var mainText = baseText.Substring(currentVersionMatch.Index + currentVersionMatch.Length,
					lastVersionMatch.Index - lastVersionMatch.Length - 1).Trim();

				var resultAdded = regexAdded.Matches(mainText);
				var resultChanged = regexChanged.Matches(mainText);
				var resultFixed = regexFixed.Matches(mainText);
				var resultRemoved = regexRemoved.Matches(mainText);

				Func<MatchCollection, List<string>> itemProcessor = (match) =>
				{
					var resultList = new List<string>();
					for (var index = 0; index < match.Count; index++)
					{
						var result = match[index];
						resultList.Add(result.Groups[2].Value.Trim());
					}
					return resultList;
				};

				ReleaseHistoryHeader = header;
				ReleaseHistoryTextAdded = itemProcessor(resultAdded);
				ReleaseHistoryTextChanged = itemProcessor(resultChanged);
				ReleaseHistoryTextFixed = itemProcessor(resultFixed);
				ReleaseHistoryTextRemoved = itemProcessor(resultRemoved);
			}
		}
		else
		{
			ReleaseHistoryHeader = "Please look the file release_history.txt";
			ReleaseHistoryTextAdded = new List<string>();
			ReleaseHistoryTextChanged = new List<string>();
			ReleaseHistoryTextFixed = new List<string>();
			ReleaseHistoryTextRemoved = new List<string>();
		}
	}
}
