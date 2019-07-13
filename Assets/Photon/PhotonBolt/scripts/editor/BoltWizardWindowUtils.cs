using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Bolt.Editor;

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
		internal static readonly string FINISH_QUESTION = "In order to finish the setup, Bolt needs to compile all Assets, do you want to proceed? If you opt to not compile now, you can run it on \"Assets/Bolt/Compile Assembly\".";

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
		SetupIntro = 1,
		SetupPhoton = 2,
		SetupBolt = 3,
		SetupSupport = 4
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

	class AccountService
	{
		readonly string REGISTER_URL = "https://www.photonengine.com/api/UnityRegister";

		public static class RegistrationConfig
		{
			public static string BOLT_SERVICE_ID = "20";
		}

		public class RegistrationInfo
		{
			public string Email { get; set; }
			public bool IsApproved { get; set; }
			public string TempId { get; set; }
			public string CultureName { get; set; }
			public Dictionary<string, string> ApplicationIds { get; set; }
			public string Tags { get; set; }
			public IList<string> TagsList { get; set; }
		}

		public class RegistrationResponse
		{
			public int ReturnCode { get; set; }
			public string Message { get; set; }
			public string MessageDetailed { get; set; }
			public bool IsSuccess { get; set; }
			public RegistrationInfo Dto { get; set; }
		}

		public class RegistrationRequest
		{
			public string Email { get; set; }
			public string ServiceTypes { get; set; }
		}

		public AccountService()
		{
			WebRequest.DefaultWebProxy = null;
			ServicePointManager.ServerCertificateValidationCallback = Validator;
		}

		public string RegisterByEmail(string email)
		{
			var result = RequestCredentials(email);

			if (!string.IsNullOrEmpty(result))
			{
				RegistrationResponse response = JsonConvert.DeserializeObject<RegistrationResponse>(result);

				if (response == null)
				{
					throw new Exception("Service temporarily unavailable. Please register through account website.");
				}

				if (response.ReturnCode == 0)
				{
					if (response.Dto.ApplicationIds.ContainsKey(RegistrationConfig.BOLT_SERVICE_ID))
					{
						return response.Dto.ApplicationIds[RegistrationConfig.BOLT_SERVICE_ID];
					}
				}
				else if (response.ReturnCode == 8)
				{
					throw new Exception(string.Format("Error: User Already exists. Please use another email or get a valid AppId from Photon dashboard."));
				}

				throw new Exception(string.Format("Error {0}: {1}", response.ReturnCode, response.Message));
			}

			throw new Exception("Server's response was empty. Please register through account website during this service interruption.");
		}

		string RequestCredentials(string email)
		{
			RegistrationRequest requestData = new RegistrationRequest
			{
				Email = email,
				ServiceTypes = RegistrationConfig.BOLT_SERVICE_ID
			};

			string body = JsonConvert.SerializeObject(requestData);

			string result = null;
			try
			{
				WebRequest webRequest = WebRequest.Create(REGISTER_URL);

				var data = Encoding.UTF8.GetBytes(body);

				webRequest.Method = "POST";
				webRequest.ContentType = "application/json";
				webRequest.ContentLength = data.Length;

				using (var writer = webRequest.GetRequestStream())
				{
					writer.Write(data, 0, data.Length);
					writer.Close();

					using (var resp = webRequest.GetResponse())
					{
						using (var reader = new StreamReader(resp.GetResponseStream()))
						{
							result = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				BoltLog.Exception(ex);
				return null;
			}

			return result;
		}

		public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true; // any certificate is ok in this case
		}
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

	static bool IsEmail(string val)
	{
		return new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").IsMatch(val);
	}
}
