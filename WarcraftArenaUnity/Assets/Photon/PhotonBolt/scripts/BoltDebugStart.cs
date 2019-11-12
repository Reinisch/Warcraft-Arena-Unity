using UdpKit;
using UnityEngine;
using Process = System.Diagnostics.Process;

public partial class BoltDebugStart : BoltInternal.GlobalEventListenerBase
{
    UdpEndPoint _serverEndPoint;
    UdpEndPoint _clientEndPoint;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = 60;
    }

    void Start()
    {
#if UNITY_EDITOR_OSX
        Process p = new Process();
        p.StartInfo.FileName = "osascript";
        p.StartInfo.Arguments =

    @"-e 'tell application """ + UnityEditor.PlayerSettings.productName + @"""
  activate
end tell'";

        p.Start();
#endif

        BoltRuntimeSettings settings = BoltRuntimeSettings.instance;

        _serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort)settings.debugStartPort);
        _clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);

        BoltConfig cfg;

        cfg = settings.GetConfigCopy();
        cfg.connectionTimeout = 60000000;
        cfg.connectionRequestTimeout = 500;
        cfg.connectionRequestAttempts = 1000;

        if (string.IsNullOrEmpty(settings.debugStartMapName) == false)
        {
            if (BoltDebugStartSettings.DebugStartIsServer)
            {
                BoltLauncher.StartServer(_serverEndPoint, cfg);
            }
            else if (BoltDebugStartSettings.DebugStartIsClient)
            {
                BoltLauncher.StartClient(_clientEndPoint, cfg);
            }

            BoltDebugStartSettings.PositionWindow();
        }
        else
        {
            BoltLog.Error("No map found to start from");
        }
    }

    public override void BoltStartFailed()
    {
        BoltLog.Error("Failed to start debug mode");
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            BoltNetwork.LoadScene(BoltRuntimeSettings.instance.debugStartMapName);
        }
        else
        {
            BoltNetwork.Connect((ushort)BoltRuntimeSettings.instance.debugStartPort);
        }
    }

    public override void SceneLoadLocalDone(string map)
    {
        Destroy(gameObject);
    }
}

