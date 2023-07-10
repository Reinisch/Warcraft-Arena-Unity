using System;
using System.Collections.Generic;
using System.IO;
using Bolt.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Bolt.Utils
{
	public static class MenuUtililies
	{
		private const string DLL_SUFIX_DEBUG = ".debug";
		private const string DLL_SUFIX_RELEASE = ".release";

		// ======= PUBLIC METHODS =====================================================================================

		[MenuItem("Bolt/Utils/Find Missing Scripts", priority = 25)]
		public static void FindMissingScriptsMenu()
		{
			BoltLog.Info("Searching for Missing Scripts");
			if (FindMissingComponents() == 0)
			{
				BoltLog.Info("Not found any prefab with missing scripts");
			}
		}

		[MenuItem("Bolt/Utils/Change DLL Mode", priority = 26)]
		public static void ChangeDllModeMenu()
		{
			var current = BoltNetwork.IsDebugMode ? "Debug" : "Release";
			var target = !BoltNetwork.IsDebugMode ? "Debug" : "Release";

			var msg = string.Format("Bolt is in {0} mode, want to change to {1}?", current, target);

			if (UnityEditor.EditorUtility.DisplayDialog("Change Bolt DLL Mode", msg, "Yes", "Cancel"))
			{
				if (ChangeDllMode())
				{
					UnityEngine.Debug.LogFormat("Bolt Mode swiched to {0}.", target);
				}
				else
				{
					UnityEngine.Debug.LogError("Error while swithing Bolt Mode, changes were reverted.");
				}
			}
		}

		public static bool ChangeDllMode()
		{
			return SwitchDebugReleaseMode(BoltNetwork.IsDebugMode);
		}

		// ======= PRIVATE METHODS =====================================================================================

		public static int FindMissingComponents()
		{
			int missingScriptsCount = 0;
			List<Component> components = new List<Component>();

			var folders = new string[] { "Assets" };
			var iter = AssetDatabase.FindAssets("t:Prefab", folders).GetEnumerator();

			while (iter.MoveNext())
			{
				var guid = (string)iter.Current;
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

				go.GetComponentsInChildren(true, components);
				for (int j = 0; j < components.Count; ++j)
				{
					if (components[j] == null)
					{
						++missingScriptsCount;
						BoltLog.Error("Missing script: " + path);
					}
				}
				components.Clear();
			}

			if (missingScriptsCount != 0)
			{
				BoltLog.Info("Found {0} Missing Scripts", missingScriptsCount);
			}

			return missingScriptsCount;
		}

		private static bool SwitchDebugReleaseMode(bool debug)
		{
			var from = debug ? DLL_SUFIX_DEBUG : DLL_SUFIX_RELEASE;
			var to = debug ? DLL_SUFIX_RELEASE : DLL_SUFIX_DEBUG;

			var paths = new string[]
			{
				BoltPathUtility.BoltDllPath,
				BoltPathUtility.BoltCompilerDLLPath,
				BoltPathUtility.BoltEditorDLLPath
			};

			var abort = false;
			var backup = "";

			foreach (var path in paths)
			{
				if (abort == true) { break; }

				try
				{
					backup = FileUtils.BackupFile(path);
					FileUtils.ExchangeFile(path, from, to);
				}
				catch (IOException ex)
				{
					Debug.LogError("Aborting...");
					Debug.LogException(ex);
					abort = true;

					try
					{
						FileUtils.BackupFile(path, true);
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
					}
				}
				finally
				{
					try
					{
						FileUtils.DeleteFile(backup);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}

					backup = "";
				}
			}

			if (abort == false)
			{
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}

			return abort == false;
		}

		private class FileUtils
		{
			public static void ExchangeFile(string basePath, string fromSuffix, string toSuffix)
			{
				MoveFile(basePath, basePath + fromSuffix);
				MoveFile(basePath + toSuffix, basePath);
			}

			public static void MoveFile(string from, string to)
			{
				if (File.Exists(to)) { return; }

				if (from.EndsWith(DLL_SUFIX_DEBUG) || from.EndsWith(DLL_SUFIX_RELEASE))
				{
					DeleteFile(string.Format("{0}.meta", from));
				}

				Debug.LogFormat("Moving file from {0} to {1}", from, to);
				File.Move(from, to);
			}

			public static string BackupFile(string path, bool restore = false)
			{
				var backup = string.Format("{0}.backup", path);

				if (restore)
				{
					Debug.LogFormat("Restore backup from file {0}", backup);
					File.Copy(backup, path, true);
				}
				else
				{
					Debug.LogFormat("Creating backup from file {0}", path);
					File.Copy(path, backup, true);
				}

				return backup;
			}

			public static void DeleteFile(string path)
			{
				if (string.IsNullOrEmpty(path) || File.Exists(path) == false) { return; }
				Debug.LogFormat("Removing file {0}", path);
				File.Delete(path);
			}
		}
	}
}