using System.Diagnostics;
using Client;
using Core;
using JetBrains.Annotations;
using Server;
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

        private enum NetworkingMode
        {
            Both,
            Server,
            Client,
        }

        [SerializeField, UsedImplicitly] private NetworkingMode mode;
        [SerializeField, UsedImplicitly] private UpdatePolicy updatePolicy;
        [SerializeField, UsedImplicitly] private long updateTimeMilliseconds = 20;

        [SerializeField, UsedImplicitly] private BalanceManager balanceManager;
        [SerializeField, UsedImplicitly] private EntityManager entityManager;
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
            HasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            HasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;

            Initialize();
        }

        [UsedImplicitly]
        private void Update()
        {
            long elapsedTime = gameTimer.ElapsedMilliseconds;
            int worldTimeDiff = (int)(elapsedTime - lastWorldUpdateTime);
            int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);

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

            lastGameUpdateTime = elapsedTime;
            DoUpdate(gameTimeDiff);
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            Deinitialize();
        }

        private void Initialize()
        {
            gameTimer.Start();

            balanceManager.Initialize();
            physicsManager.Initialize();
            entityManager.Initialize();
            multiplayerManager.Initialize();

            worldManager = HasClientLogic ? (WorldManager) new WorldClientManager() : new WorldServerManager();
            worldManager.Initialize();

            if (HasClientLogic)
            {
                inputManager.Initialize(worldManager);
                interfaceManager.Initialize();
                soundManager.Initialize();
                renderManager.Initialize(worldManager);
            }
        }

        private void Deinitialize()
        {
            if (HasClientLogic)
            {
                renderManager.Deinitialize();
                soundManager.Deinitialize();
                interfaceManager.Deinitialize();
                inputManager.Deinitialize();
            }

            worldManager.Deinitialize();

            multiplayerManager.Deinitialize();
            entityManager.Deinitialize();
            physicsManager.Deinitialize();
            balanceManager.Deinitialize();
        }

        private void DoUpdate(int deltaTime)
        {
            multiplayerManager.DoUpdate(deltaTime);

            if (HasClientLogic)
            {
                renderManager.DoUpdate(deltaTime);
                interfaceManager.DoUpdate(deltaTime);
            }
        }
    }
}