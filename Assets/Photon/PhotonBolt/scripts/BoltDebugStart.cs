using System.Collections;
using Bolt;
using UdpKit;
using UnityEngine;
using Process = System.Diagnostics.Process;

public partial class BoltDebugStart : GlobalEventListener
{
	void Awake()
	{
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

		var settings = BoltRuntimeSettings.instance;

		var cfg = settings.GetConfigCopy();
		cfg.connectionTimeout = 60000000;
		cfg.connectionRequestTimeout = 500;
		cfg.connectionRequestAttempts = 1000;

		if (string.IsNullOrEmpty(settings.debugStartMapName) == false)
		{
			if (BoltDebugStartSettings.DebugStartIsServer)
			{
				BoltLog.Warn("Starting as SERVER");

				var serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort)settings.debugStartPort);

				BoltLauncher.StartServer(serverEndPoint, cfg);
			}
			else if (BoltDebugStartSettings.DebugStartIsClient)
			{
				BoltLog.Warn("Starting as CLIENT");

				var clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);

				BoltLauncher.StartClient(clientEndPoint, cfg);
			}
			else if (BoltDebugStartSettings.DebugStartIsSinglePlayer)
			{
				BoltLog.Warn("Starting as SINGLE PLAYER");

				BoltLauncher.StartSinglePlayer(cfg);
			}

			BoltDebugStartSettings.PositionWindow();
		}
		else
		{
			BoltLog.Error("No map found to start from");
		}
	}

	public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
	{
		BoltLog.Error("Failed to start debug mode");
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer || BoltNetwork.IsSinglePlayer)
		{
			BoltNetwork.LoadScene(BoltRuntimeSettings.instance.debugStartMapName);
		}
		else if (BoltNetwork.IsClient)
		{
			StartCoroutine(DelayClientConnect());
		}
	}

	private IEnumerator DelayClientConnect()
	{
		for (int i = 0; i < 5; i++)
		{
			BoltLog.Info("Connecting in {0} seconds...", 5 - i);
			yield return new WaitForSeconds(1);
		}

		BoltNetwork.Connect((ushort)BoltRuntimeSettings.instance.debugStartPort);
	}
}
