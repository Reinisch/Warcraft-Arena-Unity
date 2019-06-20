using System;
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

        [SerializeField, UsedImplicitly] private MultiplayerManager multiplayerManager;
        [SerializeField, UsedImplicitly] private InterfaceManager interfaceManager;
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

            if (worldManager != null)
            {
                switch (updatePolicy)
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
            }
            else
            {
                lastWorldUpdateTime = elapsedTime;
            }

            lastGameUpdateTime = elapsedTime;

            multiplayerManager.DoUpdate(gameTimeDiff);

            if (HasClientLogic)
            {
                scriptableClientContainer.DoUpdate(gameTimeFloatDiff);
                interfaceManager.DoUpdate(gameTimeFloatDiff);
            }
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            Deinitialize();
        }

        private void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            Assert.RaiseExceptions = Application.isEditor;

            scriptableCoreContainer.Register();
            multiplayerManager.Initialize();
            scriptableClientContainer.Register();
            interfaceManager.Initialize(multiplayerManager, multiplayerManager.ClientListener);

            gameTimer.Start();

            interfaceManager.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(true));

            EventHandler.RegisterEvent<string, NetworkingMode>(multiplayerManager, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.RegisterEvent<UdpConnectionDisconnectReason>(multiplayerManager, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
            EventHandler.RegisterEvent(multiplayerManager, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
        }

        private void Deinitialize()
        {
            EventHandler.UnregisterEvent(multiplayerManager, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
            EventHandler.UnregisterEvent<string, NetworkingMode>(multiplayerManager, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.UnregisterEvent<UdpConnectionDisconnectReason>(multiplayerManager, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);

            if (worldManager != null)
                DeinitializeWorld();

            interfaceManager.Deinitialize();
            scriptableClientContainer.Unregister();
            multiplayerManager.Deinitialize();
            scriptableCoreContainer.Unregister();
        }

        private void InitializeWorld()
        {
            worldManager = HasServerLogic ? (WorldManager) new WorldServerManager(HasClientLogic) : new WorldClientManager(HasServerLogic);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, worldManager);
        }

        private void DeinitializeWorld()
        {
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, worldManager);

            worldManager.Dispose();
            worldManager = null;
        }

        private void OnGameMapLoaded(string map, NetworkingMode mode)
        {
            if (mode == NetworkingMode.None)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Game loaded with invalid networking state!");

            HasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            HasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;

            InitializeWorld();

            interfaceManager.HideScreen<LobbyScreen>();
            interfaceManager.ShowScreen<BattleScreen, BattleHudPanel>();
        }

        private void OnDisconnectedFromMaster()
        {
            ProcessDisconnect(false, DisconnectReason.DisconnectedFromMaster);
        }

        private void OnDisconnectedFromHost(UdpConnectionDisconnectReason udpDisconnectReason)
        {
            ProcessDisconnect(true, udpDisconnectReason.ToDisconnectReason());
        }

        private void ProcessDisconnect(bool autoStartClient, DisconnectReason disconnectReason)
        {
            DeinitializeWorld();

            HasServerLogic = false;
            HasClientLogic = false;

            interfaceManager.HideScreen<BattleScreen>();
            interfaceManager.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(autoStartClient, disconnectReason));
        }
    }
}