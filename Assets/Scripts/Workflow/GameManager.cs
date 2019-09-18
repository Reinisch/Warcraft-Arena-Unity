using System.Diagnostics;
using Common;
using Core;
using JetBrains.Annotations;
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

        [SerializeField, UsedImplicitly] private UpdatePolicy updatePolicy;
        [SerializeField, UsedImplicitly] private long updateTimeMilliseconds = 20;
        [SerializeField, UsedImplicitly] private ScriptableContainer scriptableCoreContainer;
        [SerializeField, UsedImplicitly] private ScriptableContainer scriptableClientContainer;

        private readonly Stopwatch gameTimer = new Stopwatch();
        private WorldManager world;
        private long lastWorldUpdateTime;
        private long lastGameUpdateTime;

        [UsedImplicitly]
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            scriptableCoreContainer.Register();
            scriptableClientContainer.Register();

            gameTimer.Start();

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        [UsedImplicitly]
        private void Update()
        {
            long elapsedTime = gameTimer.ElapsedMilliseconds;
            int worldTimeDiff = (int)(elapsedTime - lastWorldUpdateTime);
            int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);
            float gameTimeFloatDiff = gameTimeDiff / 1000.0f;

            lastGameUpdateTime = elapsedTime;

            if (world == null)
                lastWorldUpdateTime = elapsedTime;
            else switch (updatePolicy)
            {
                case UpdatePolicy.EveryUpdateCall:
                    lastWorldUpdateTime = elapsedTime;
                    world.DoUpdate(worldTimeDiff);
                    break;
                case UpdatePolicy.FixedTimeDelta:
                    if (worldTimeDiff >= updateTimeMilliseconds)
                        goto case UpdatePolicy.EveryUpdateCall;
                    break;
                default:
                    goto case UpdatePolicy.EveryUpdateCall;
            }

            scriptableCoreContainer.DoUpdate(gameTimeDiff);

            if (world != null && world.HasClientLogic)
                scriptableClientContainer.DoUpdate(gameTimeFloatDiff);
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            world?.Dispose();
            scriptableClientContainer.Unregister();
            scriptableCoreContainer.Unregister();
        }

        private void OnWorldInitialized(WorldManager initializedWorld)
        {
            world = initializedWorld;
        }

        private void OnWorldDeinitializing(WorldManager deinitializingWorld)
        {
            world = null;
        }
    }
}