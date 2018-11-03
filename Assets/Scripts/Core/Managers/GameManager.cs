using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class GameManager : SingletonGameObject<GameManager>
    {
        private enum UpdatePolicy
        {
            EveryUpdateCall,
            FixedTimeDelta
        }

        private enum GameMultiplayerMode
        {
            Both,
            Server,
            Client,
            Debug
        }

        [SerializeField, UsedImplicitly] private GameMultiplayerMode mode;
        [SerializeField, UsedImplicitly] private UpdatePolicy updatePolicy;
        [SerializeField, UsedImplicitly] private long updateTimeMilliseconds = 20;

        [SerializeField, UsedImplicitly] private List<SingletonGameObject> coreManagers;
        [SerializeField, UsedImplicitly] private List<SingletonGameObject> clientManagers;
        [SerializeField, UsedImplicitly] private List<SingletonGameObject> serverManagers;

        private readonly Stopwatch gameTimer = new Stopwatch();
        private long lastWorldUpdateTime;
        private long lastGameUpdateTime;

        public bool HasServerLogic { get; private set; }
        public bool HasClientLogic { get; private set; }
        public bool IsDebugLogic { get; private set; }

        public Guid LocalPlayerId { get; private set; }
        public long LastGameUpdateTime => lastGameUpdateTime;

        protected override void Awake()
        {
            base.Awake();

            IsDebugLogic = mode == GameMultiplayerMode.Debug;
            HasServerLogic = mode == GameMultiplayerMode.Server || mode == GameMultiplayerMode.Both || IsDebugLogic;
            HasClientLogic = mode == GameMultiplayerMode.Client || mode == GameMultiplayerMode.Both || IsDebugLogic;

            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
                Instance.Initialize();
            }
        }

        private void Start()
        {
            gameTimer.Start();
        }

        private void Update()
        {
            long elapsedTime = gameTimer.ElapsedMilliseconds;
            int worldTimeDiff = (int)(elapsedTime - lastWorldUpdateTime);
            int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);

            switch (updatePolicy)
            {
                case UpdatePolicy.EveryUpdateCall:
                    lastWorldUpdateTime = elapsedTime;
                    World.Instance.DoUpdate(worldTimeDiff);
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

        private void OnApplicationQuit()
        {
            Deinitialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            coreManagers.ForEach(manager => manager.Initialize());

            if (HasServerLogic)
                serverManagers.ForEach(manager => manager.Initialize());

            World.Instance.Initialize();

            if (IsDebugLogic)
            {
                Map mainMap = MapManager.Instance.FindMap(1);
                Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
                Player localPlayer = EntityManager.Instance.SpawnEntity<Player>(EntityType.Player, new EntitySpawnData(spawnPoint.position, spawnPoint.rotation));
                localPlayer.SetMap(mainMap);
                LocalPlayerId = localPlayer.Guid;
            }

            if (HasClientLogic)
                clientManagers.ForEach(manager => manager.Initialize());
        }

        public override void Deinitialize()
        {
            if (HasClientLogic)
            {
                clientManagers.ForEach(manager => manager.Deinitialize());
                clientManagers.Clear();
            }

            if(HasServerLogic)
            {
                serverManagers.ForEach(manager => manager.Deinitialize());
                serverManagers.Clear();
            }

            World.Instance.Deinitialize();

            coreManagers.ForEach(manager => manager.Deinitialize());
            coreManagers.Clear();

            base.Deinitialize();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            coreManagers.ForEach(manager => manager.DoUpdate(deltaTime));
            if (HasServerLogic)
                serverManagers.ForEach(manager => manager.DoUpdate(deltaTime));
            if (HasClientLogic)
                clientManagers.ForEach(manager => manager.DoUpdate(deltaTime));
        }
    }
}