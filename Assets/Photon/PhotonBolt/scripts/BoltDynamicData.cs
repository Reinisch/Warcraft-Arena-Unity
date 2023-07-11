using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoltInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Bolt
{
	[Preserve]
	public static class BoltDynamicData
	{
		public static void Setup()
		{
			BoltNetworkInternal.DebugDrawer = new BoltInternal.UnityDebugDrawer();

#if UNITY_PRO_LICENSE
			BoltNetworkInternal.UsingUnityPro = true;
#else
			BoltNetworkInternal.UsingUnityPro = false;
#endif

			BoltNetworkInternal.GetActiveSceneIndex = GetActiveSceneIndex;
			BoltNetworkInternal.GetSceneName = GetSceneName;
			BoltNetworkInternal.GetSceneIndex = GetSceneIndex;
			BoltNetworkInternal.GetGlobalBehaviourTypes = GetGlobalBehaviourTypes;
			BoltNetworkInternal.EnvironmentSetup = BoltInternal.BoltNetworkInternal_User.EnvironmentSetup;
			BoltNetworkInternal.EnvironmentReset = BoltInternal.BoltNetworkInternal_User.EnvironmentReset;

			// Setup Unity Config

#if ENABLE_IL2CPP
			UnitySettings.IsBuildIL2CPP = true;
#elif ENABLE_MONO
			UnitySettings.IsBuildMono = true;
#elif ENABLE_DOTNET
			UnitySettings.IsBuildDotNet = true;
#endif

			UnitySettings.CurrentPlatform = Application.platform;
		}

		private static int GetActiveSceneIndex()
		{
			return GetSceneIndex(SceneManager.GetActiveScene().name);
		}

		private static int GetSceneIndex(string name)
		{
			try
			{
				return BoltScenes_Internal.GetSceneIndex(name);
			}
			catch
			{
				return -1;
			}
		}

		private static string GetSceneName(int index)
		{
			try
			{
				return BoltScenes_Internal.GetSceneName(index);
			}
			catch
			{
				return null;
			}
		}

		private static List<STuple<BoltGlobalBehaviourAttribute, Type>> GetGlobalBehaviourTypes()
		{
			List<STuple<BoltGlobalBehaviourAttribute, Type>> result = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();

			try
			{
				var asmIter = BoltAssemblies.AllAssemblies;
				while (asmIter.MoveNext())
				{
					var asm = GetAssemblyByName(asmIter.Current);
					if (asm == null) { continue; }

					foreach (Type type in asm.GetTypes())
					{
						if (typeof(MonoBehaviour).IsAssignableFrom(type))
						{
							var attrs = (BoltGlobalBehaviourAttribute[])type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);

							if (attrs.Length == 1)
							{
								result.Add(new STuple<BoltGlobalBehaviourAttribute, Type>(attrs[0], type));
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				BoltLog.Exception(e);
			}

			return result;
		}

		private static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
		}
	}
}
