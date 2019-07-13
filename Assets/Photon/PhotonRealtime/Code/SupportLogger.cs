// ----------------------------------------------------------------------------
// <copyright file="SupportLogger.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Implements callbacks of the Realtime API to logs selected information
//   for support cases.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Stopwatch = System.Diagnostics.Stopwatch;

    #if SUPPORTED_UNITY
    using UnityEngine;
    #endif

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif

    /// <summary>
    /// Helper class to debug log basic information about Photon client and vital traffic statistics.
    /// </summary>
    /// <remarks>
    /// Set SupportLogger.Client for this to work.
    /// </remarks>
    #if SUPPORTED_UNITY
	[AddComponentMenu("")] // hide from Unity Menus and searches
	public class SupportLogger : MonoBehaviour, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks
    #else
	public class SupportLogger : IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks , ILobbyCallbacks
    #endif
    {
        /// <summary>
        /// Toggle to enable or disable traffic statistics logging.
        /// </summary>
        public bool LogTrafficStats = true;
        private bool loggedStillOfflineMessage;

        private LoadBalancingClient client;

        private Stopwatch startStopwatch;
        
        private int pingMax;
        private int pingMin;

        /// <summary>
        /// Photon client to log information and statistics from.
        /// </summary>
        public LoadBalancingClient Client
        {
            get { return this.client; }
            set
            {
                if (this.client != value)
                {
                    if (this.client != null)
                    {
                        this.client.RemoveCallbackTarget(this);
                    }
                    this.client = value;
                    this.client.AddCallbackTarget(this);
                }
            }
        }


        #if SUPPORTED_UNITY
        protected void Start()
        {
            if (this.startStopwatch == null)
            {
                this.startStopwatch = new Stopwatch();
                this.startStopwatch.Start();
            }

            this.InvokeRepeating("TrackValues", 0.5f, 0.5f);
        }

        protected void OnApplicationPause(bool pause)
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnApplicationPause: " + pause + " connected: " + this.client.IsConnected);
        }

        protected void OnApplicationQuit()
        {
            this.CancelInvoke();
        }
        #endif

        private string GetFormattedTimestamp()
        {
            if (this.startStopwatch == null)
            {
                this.startStopwatch = new Stopwatch();
                this.startStopwatch.Start();
            }
            return string.Format("[{0}.{1}]", this.startStopwatch.Elapsed.Seconds, this.startStopwatch.Elapsed.Milliseconds);
        }

        
        // called via InvokeRepeatedly
        private void TrackValues()
        {
            int currentRtt = this.client.LoadBalancingPeer.RoundTripTime;
            if (currentRtt > this.pingMax)
            {
                this.pingMax = currentRtt;
            }
            if (currentRtt < this.pingMin)
            {
                this.pingMin = currentRtt;
            }
        }


        /// <summary>
        /// Debug logs vital traffic statistics about the attached Photon Client.
        /// </summary>
        public void LogStats()
        {
            if (this.client.State == ClientState.PeerCreated)
            {
                return;
            }

            if (this.LogTrafficStats)
            {
                Debug.Log(this.GetFormattedTimestamp() + " SupportLogger " + this.client.LoadBalancingPeer.VitalStatsToString(false) + " Ping min/max: " + this.pingMin + "/" + this.pingMax);
            }
        }

        /// <summary>
        /// Debug logs basic information (AppId, AppVersion, PeerID, Server address, Region) about the attached Photon Client.
        /// </summary>
        private void LogBasics()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} SupportLogger Info: ", this.GetFormattedTimestamp());
            sb.AppendFormat("AppID: \"{0}\" AppVersion: \"{1}\" PeerID: {2} ",
                string.IsNullOrEmpty(this.client.AppId) || this.client.AppId.Length < 8
                    ? this.client.AppId
                    : string.Concat(this.client.AppId.Substring(0, 8), "***"), this.client.AppVersion, this.client.LoadBalancingPeer.PeerID);
            //NOTE: this.client.LoadBalancingPeer.ServerIpAddress requires Photon3Unity3d.dll v4.1.2.5 and up
            sb.AppendFormat("NameServer: {0} Server: {1} IP: {2} Region: {3}", this.client.NameServerHost, this.client.CurrentServerAddress, this.client.LoadBalancingPeer.ServerIpAddress, this.client.CloudRegion);

            Debug.Log(sb.ToString());
        }


        public void OnConnected()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnected().");
            this.pingMax = 0;
            this.pingMin = this.client.LoadBalancingPeer.RoundTripTime;
            this.LogBasics();

            if (this.LogTrafficStats)
            {
                this.client.LoadBalancingPeer.TrafficStatsEnabled = false;
                this.client.LoadBalancingPeer.TrafficStatsEnabled = true;
                this.InvokeRepeating("LogStats", 10, 10);
            }
        }

        public void OnConnectedToMaster()
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnectedToMaster().");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnFriendListUpdate(friendList).");
        }

        public void OnJoinedLobby()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinedLobby(" + this.client.CurrentLobby + ").");
        }

        public void OnLeftLobby()
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftLobby().");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCreateRoomFailed(" + returnCode+","+message+").");
        }

        public void OnJoinedRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinRandomFailed(" + returnCode+","+message+").");
        }

        public void OnCreatedRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCreatedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnLeftRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftRoom().");
        }

		public void OnDisconnected(DisconnectCause cause)
        {
            this.CancelInvoke("LogStats");

			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnDisconnected(" + cause + ").");
			this.LogBasics();
            this.LogStats();
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRegionListReceived(regionHandler).");
            this.LogBasics();
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerEnteredRoom(" + newPlayer+").");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerLeftRoom(" + otherPlayer+").");
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnMasterClientSwitched(" + newMasterClient+").");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationResponse(" + data.ToStringFull()+").");
        }

		public void OnCustomAuthenticationFailed (string debugMessage)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationFailed(" + debugMessage+").");
		}

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
        }


        #if !SUPPORTED_UNITY
        private static class Debug
        {
            public static void Log(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogWarning(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogError(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }
        #endif
    }
}