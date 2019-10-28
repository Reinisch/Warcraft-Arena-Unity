using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bolt.Editor;
using UnityEditor;
using UnityEngine;

namespace Bolt.Utils
{
	public static class MenuUtililies
	{
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

			if (EditorUtility.DisplayDialog("Change Bolt DLL Mode", msg, "Yes", "Cancel"))
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
				var guid = (string) iter.Current;
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
			var from = debug ? ".debug" : ".release";
			var to = debug ? ".release" : ".debug";

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
				catch (IOException)
				{
					FileUtils.BackupFile(path, true);
					abort = true;
				}
				finally
				{
					FileUtils.DeleteFile(backup);
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

				UnityEngine.Debug.LogFormat("Moving file from {0} to {1}", from, to);
				File.Move(from, to);
			}

			public static string BackupFile(string path, bool restore = false)
			{
				var backup = path + ".backup";

				if (restore)
				{
					File.Copy(backup, path, true);
				}
				else
				{
					File.Copy(path, backup, true);
				}

				return backup;
			}

			public static void DeleteFile(string path)
			{
				if (string.IsNullOrEmpty(path) || File.Exists(path) == false) { return; }
				File.Delete(path);
			}
		}
	}
}