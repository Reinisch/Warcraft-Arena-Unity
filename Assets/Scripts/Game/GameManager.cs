using System.Diagnostics;
using Client;
using Common;
using Core;
using JetBrains.Annotations;
using Server;
using UdpKit;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private enum UpdatePolicy
        {
            EveryUpdateCall,
            FixedTimeDelta
        }

        public enum NetworkingMode
        {
            None,
            Both,
            Server,
            Client,
        }

        [SerializeField, UsedImplicitly] private UpdatePolicy updatePolicy;
        [SerializeField, UsedImplicitly] private long updateTimeMilliseconds = 20;
        [SerializeField, UsedImplicitly] private InterfaceReference interfaceReference;
        [SerializeField, UsedImplicitly] private ScriptableContainer scriptableCoreContainer;
        [SerializeField, UsedImplicitly] private ScriptableContainer scriptableClientContainer;

        private readonly Stopwatch gameTimer = new Stopwatch();
        private WorldManager worldManager;
        private long lastWorldUpdateTime;
        private long lastGameUpdateTime;

        private bool HasServerLogic { get; set; }
        private bool HasClientLogic { get; set; }

        [UsedImplicitly]
        private void Awake()
        {
            Initialize();
        }

        [UsedImplicitly]
        private void Update()
        {
            long elapsedTime = gameTimer.ElapsedMilliseconds;
            int worldTimeDiff = (int)(elapsedTime - lastWorldUpdateTime);
            int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);
            float gameTimeFloatDiff = gameTimeDiff / 1000.0f;

            lastGameUpdateTime = elapsedTime;

            if (worldManager == null)
                lastWorldUpdateTime = elapsedTime;
            else switch (updatePolicy)
            {
                case UpdatePolicy.EveryUpdateCall:
                    lastWorldUpdateTime = elapsedTime;
                    worldManager.DoUpdate(worldTimeDiff);
                    break;
                case UpdatePolicy.FixedTimeDelta:
                    if (worldTimeDiff >= updateTimeMilliseconds)
                        goto case UpdatePolicy.EveryUpdateCall;
                    break;
                default:
                    goto case UpdatePolicy.EveryUpdateCall;
            }

            scriptableCoreContainer.DoUpdate(gameTimeDiff);

            if (HasClientLogic)
                scriptableClientContainer.DoUpdate(gameTimeFloatDiff);
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            Deinitialize();
        }

        private void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            Assert.RaiseExceptions = Application.isEditor || UnityEngine.Debug.isDebugBuild;

            scriptableCoreContainer.Register();
            scriptableClientContainer.Register();

            gameTimer.Start();

            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(true));

            EventHandler.RegisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.RegisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
            EventHandler.RegisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
        }

        private void Deinitialize()
        {
            EventHandler.UnregisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
            EventHandler.UnregisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.UnregisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);

            worldManager?.Dispose();
            scriptableClientContainer.Unregister();
            scriptableCoreContainer.Unregister();
        }

        private void OnGameMapLoaded(string map, NetworkingMode mode)
        {
            HasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            HasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;

            worldManager = HasServerLogic ? (WorldManager)new WorldServerManager(HasClientLogic) : new WorldClientManager(HasServerLogic);
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, worldManager);

            interfaceReference.HideScreen<LobbyScreen>();
            interfaceReference.ShowScreen<BattleScreen, BattleHudPanel>();
        }

        private void OnDisconnectedFromMaster()
        {
            ProcessDisconnect(DisconnectReason.DisconnectedFromMaster);
        }

        private void OnDisconnectedFromHost(UdpConnectionDisconnectReason udpDisconnectReason)
        {
            ProcessDisconnect(udpDisconnectReason.ToDisconnectReason());
        }

        private void ProcessDisconnect(DisconnectReason disconnectReason)
        {
            worldManager.Dispose();
            worldManager = null;

            HasServerLogic = false;
            HasClientLogic = false;

            interfaceReference.HideScreen<BattleScreen>();
            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(false, disconnectReason));
        }
    }
}