using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdpKit;
using UnityEngine;

namespace Bolt
{
	public static class BoltDynamicData
	{
		static readonly string ASSEMBLY_CSHARP = "Assembly-CSharp";
		static readonly string ASSEMBLY_CSHARP_FIRST = "Assembly-CSharp-firstpass";

		public static void Setup()
		{
			BoltNetworkInternal.DebugDrawer = new BoltInternal.UnityDebugDrawer();

#if UNITY_PRO_LICENSE
			BoltNetworkInternal.UsingUnityPro = true;
#else
			BoltNetworkInternal.UsingUnityPro = false;
#endif

			BoltNetworkInternal.GetSceneName = GetSceneName;
			BoltNetworkInternal.GetSceneIndex = GetSceneIndex;
			BoltNetworkInternal.GetGlobalBehaviourTypes = GetGlobalBehaviourTypes;
			BoltNetworkInternal.EnvironmentSetup = BoltInternal.BoltNetworkInternal_User.EnvironmentSetup;
			BoltNetworkInternal.EnvironmentReset = BoltInternal.BoltNetworkInternal_User.EnvironmentReset;
		}

		static int GetSceneIndex(string name)
		{
			return BoltInternal.BoltScenes_Internal.GetSceneIndex(name);
		}

		static string GetSceneName(int index)
		{
			return BoltInternal.BoltScenes_Internal.GetSceneName(index);
		}

		static public List<STuple<BoltGlobalBehaviourAttribute, Type>> GetGlobalBehaviourTypes()
		{
#if UNITY_WSA
			Assembly[] assemblies = { typeof(BoltLauncher).GetTypeInfo().Assembly };
#else
			Assembly[] assemblies = { GetAssemblyByName(ASSEMBLY_CSHARP), GetAssemblyByName(ASSEMBLY_CSHARP_FIRST) };
#endif

			List<STuple<BoltGlobalBehaviourAttribute, Type>> result = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();

			try
			{
				foreach (Assembly asm in assemblies)
				{
					if (asm == null) { continue; }

					foreach (Type type in asm.GetTypes())
					{
						if (typeof(MonoBehaviour).IsAssignableFrom(type))
						{
#if UNITY_WSA
							var attrs = (BoltGlobalBehaviourAttribute[]) type.GetTypeInfo().GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
#else
							var attrs = (BoltGlobalBehaviourAttribute[]) type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
#endif
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

		static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
		}
	}
}