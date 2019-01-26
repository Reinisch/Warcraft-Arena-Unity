using System;
using System.Diagnostics;
using Client;
using Core;
using JetBrains.Annotations;
using Server;
using UdpKit;
using UnityEngine;

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

        [SerializeField, UsedImplicitly] private BalanceManager balanceManager;
        [SerializeField, UsedImplicitly] private PhysicsManager physicsManager;
        [SerializeField, UsedImplicitly] private MultiplayerManager multiplayerManager;

        [SerializeField, UsedImplicitly] private InputManager inputManager;
        [SerializeField, UsedImplicitly] private InterfaceManager interfaceManager;
        [SerializeField, UsedImplicitly] private SoundManager soundManager;
        [SerializeField, UsedImplicitly] private RenderManager renderManager;

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
                renderManager.DoUpdate(gameTimeDiff);
                interfaceManager.DoUpdate(gameTimeDiff);
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

            gameTimer.Start();

            balanceManager.Initialize();
            physicsManager.Initialize();
            multiplayerManager.Initialize();
            interfaceManager.Initialize(multiplayerManager, multiplayerManager.ClientListener);

            multiplayerManager.EventGameMapLoaded += OnGameMapLoaded;
            multiplayerManager.EventDisconnectedFromHost += OnDisconnectedFromHost;

            interfaceManager.HideBattleScreen();
            interfaceManager.ShowLobbyScreen(true);
        }

        private void Deinitialize()
        {
            multiplayerManager.EventGameMapLoaded -= OnGameMapLoaded;
            multiplayerManager.EventDisconnectedFromHost -= OnDisconnectedFromHost;

            if (worldManager != null)
            {
                if (HasClientLogic)
                {
                    renderManager.Deinitialize();
                    soundManager.Deinitialize();
                    inputManager.Deinitialize();
                }
                
                worldManager.Dispose();
            }

            worldManager = null;

            interfaceManager.Deinitialize();
            multiplayerManager.Deinitialize();
            physicsManager.Deinitialize();
            balanceManager.Deinitialize();
        }

        private void OnGameMapLoaded(string map, NetworkingMode mode)
        {
            if (mode == NetworkingMode.None)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Game loaded with invalid networking state!");

            HasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            HasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;
            worldManager = HasServerLogic ? (WorldManager) new WorldServerManager() : new WorldClientManager();

            if (HasClientLogic)
            {
                renderManager.Initialize(worldManager);
                soundManager.Initialize(worldManager);
                inputManager.Initialize(worldManager);
                interfaceManager.InitializeWorld(worldManager);
            }

            multiplayerManager.InitializeWorld(worldManager, HasServerLogic, HasClientLogic);

            interfaceManager.HideLobbyScreen();
            interfaceManager.ShowBattleScreen();
        }

        private void OnDisconnectedFromHost(UdpConnectionDisconnectReason reason)
        {
            multiplayerManager.DeinitializeWorld(HasServerLogic, HasClientLogic);

            if (HasClientLogic)
            {
                interfaceManager.DeinitializeWorld();
                renderManager.Deinitialize();
                soundManager.Deinitialize();
                inputManager.Deinitialize();
            }

            HasServerLogic = false;
            HasClientLogic = false;
            worldManager.Dispose();
            worldManager = null;

            interfaceManager.HideBattleScreen();
            interfaceManager.ShowLobbyScreen(false);
            interfaceManager.LobbyScreen.SetStatusDisconnectDescription(reason);
        }
    }
}