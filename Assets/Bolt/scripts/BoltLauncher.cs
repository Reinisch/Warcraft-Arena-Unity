using System;
using System.Collections.Generic;
using System.Reflection;
using UdpKit;
using UnityEngine;

/// <summary>
/// Bolt launcher.
/// 
/// Use this class to quickly initialize your Bolt peer using the utility 
/// functions to start in Server, Client and Single Player modes.
/// </summary>
public static class BoltLauncher
{
    static UdpPlatform TargetPlatform
    {
        get; set;
    }

    // SINGLE PLAYER

    /// <summary>
    /// Starts Bolt as a Single Player game
    /// </summary>
    /// <param name="config">Custom Bolt configuration</param>
    public static void StartSinglePlayer(BoltConfig config = null)
    {
        if (config == null)
        {
            config = BoltRuntimeSettings.instance.GetConfigCopy();
        }

        // init server
        Initialize(BoltNetworkModes.None, UdpEndPoint.Any, config);
    }

    // START SERVER

    /// <summary>
    /// Starts Bolt as Server.
    /// </summary>
    /// <param name="port">Port where the Server will try to bind</param>
    public static void StartServer(int port = -1)
    {
        if (port >= 0 && port <= ushort.MaxValue)
        {
            StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)port));
        }
        else if (port == -1)
        {
            StartServer(UdpEndPoint.Any);
        }
        else
        {
            throw new ArgumentOutOfRangeException(string.Format("'port' must be >= 0 and <= {0}", ushort.MaxValue));
        }
    }

    /// <summary>
    /// Starts Bolt as Server.
    /// </summary>
    /// <param name="config">Custom Bolt configuration</param>
    /// <param name="scene">Default Scene loaded by Bolt when the initialization is complete</param>
    public static void StartServer(BoltConfig config, string scene = null)
    {
        StartServer(UdpEndPoint.Any, config, scene);
    }

    /// <summary>
    /// Starts Bolt as Server.
    /// </summary>
    /// <param name="endpoint">Custom EndPoint where Bolt will try to bind</param>
    /// <param name="scene">Default Scene loaded by Bolt when the initialization is complete</param>
    public static void StartServer(UdpEndPoint endpoint, string scene = null)
    {
        StartServer(endpoint, BoltRuntimeSettings.instance.GetConfigCopy(), scene);
    }

    /// <summary>
    /// Starts Bolt as Server.
    /// </summary>
    /// <param name="endpoint">Custom EndPoint where Bolt will try to bind</param>
    /// <param name="config">Custom Bolt configuration</param>
    /// <param name="scene">Default Scene loaded by Bolt when the initialization is complete</param>
    public static void StartServer(UdpEndPoint endpoint, BoltConfig config, string scene = null)
    {
        Initialize(BoltNetworkModes.Server, endpoint, config, scene);
    }

    // START CLIENT

    /// <summary>
    /// Starts Bolt as Client.
    /// </summary>
    /// <param name="port">Port where the Server will try to bind</param>
    public static void StartClient(int port = -1)
    {
        if (port >= 0 && port <= ushort.MaxValue)
        {
            StartClient(new UdpEndPoint(UdpIPv4Address.Any, (ushort)port));
        }
        else if (port == -1)
        {
            StartClient(UdpEndPoint.Any);
        }
        else
        {
            throw new ArgumentOutOfRangeException(string.Format("'port' must be >= 0 and <= {0}", ushort.MaxValue));
        }
    }

    /// <summary>
    /// Starts Bolt as Client.
    /// </summary>
    /// <param name="config">Custom Bolt configuration</param>
    public static void StartClient(BoltConfig config)
    {
        StartClient(UdpEndPoint.Any, config);
    }

    /// <summary>
    /// Starts Bolt as Client.
    /// </summary>
    /// <param name="endpoint">Custom EndPoint where Bolt will try to bind</param>
    /// <param name="config">Custom Bolt configuration</param>
    public static void StartClient(UdpEndPoint endpoint, BoltConfig config = null)
    {
        if (config == null)
        {
            config = BoltRuntimeSettings.instance.GetConfigCopy();
        }

        Initialize(BoltNetworkModes.Client, endpoint, config);
    }

    // Utils

    /// <summary>
    /// Utility function to initialize Bolt with the specified modes, endpoint, config and scene.
    /// </summary>
    /// <param name="modes">Bolt mode. <see cref="BoltNetworkModes"/></param>
    /// <param name="endpoint">Custom EndPoint where Bolt will try to bind</param>
    /// <param name="config">Custom Bolt configuration</param>
    /// <param name="scene">Default Scene loaded by Bolt when the initialization is complete</param>
    static void Initialize(BoltNetworkModes modes, UdpEndPoint endpoint, BoltConfig config, string scene = null)
    {
        BoltDynamicData.Setup();
        BoltNetworkInternal.Initialize(modes, endpoint, config, TargetPlatform, scene);
    }

    /// <summary>
    /// Shutdown this Bolt instance.
    /// </summary>
    public static void Shutdown()
    {
        BoltNetworkInternal.Shutdown();
    }

    // Platform Settings

    /// <summary>
    /// Set a custom UDP platform. Use this method only to set custom properties
    /// to your desired platform. By default, there is no need to change 
    /// the platform, this is handled internally by Bolt. 
    /// </summary>
    /// <param name="platform">Custom UdpPlatform</param>
    /// <example>
    /// This example show how to set a custom PhotonPlatform:
    /// <code>
    /// BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig
    /// {
    ///     AppId = "your-app-id",
    ///     RegionMaster = "your-region",
    ///     UsePunchThrough = true, // set to false, to disable PunchThrough
    ///     MaxConnections = 32
    /// }));
    /// </code>
    /// </example>
    public static void SetUdpPlatform(UdpPlatform platform)
    {
        TargetPlatform = platform;
    }
}

static class BoltDynamicData
{
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
        Assembly asm = typeof(BoltLauncher).GetTypeInfo().Assembly;
#else
        Assembly asm = Assembly.GetExecutingAssembly();
#endif

        List<STuple<BoltGlobalBehaviourAttribute, Type>> result = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();

        try
        {
            foreach (Type type in asm.GetTypes())
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
#if UNITY_WSA
          var attrs = (BoltGlobalBehaviourAttribute[])type.GetTypeInfo().GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
#else
                    var attrs = (BoltGlobalBehaviourAttribute[])type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
#endif
                    if (attrs.Length == 1)
                    {
                        result.Add(new STuple<BoltGlobalBehaviourAttribute, Type>(attrs[0], type));
                    }
                }
            }
        }
        catch (Exception)
        {
        }

        return result;
    }
}