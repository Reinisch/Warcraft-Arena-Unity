using System.Diagnostics;
using Assets.Scripts.Workflow;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class WorldTickableManager : MonoBehaviour
    {
        private enum UpdatePolicy
        {
            EveryUpdateCall,
            FixedTimeDelta
        }

        [SerializeField]
        private UpdatePolicy updatePolicy;

        [SerializeField]
        private long updateTimeMilliseconds = 20;

        [Inject]
        private WorldSession worldSession;

        [Inject]
        private World world;

        private readonly Stopwatch gameTimer = new();
        private long lastWorldUpdateTime;
        private long lastGameUpdateTime;

        [UsedImplicitly]
        private void Awake()
        {
            gameTimer.Start();
        }

        [UsedImplicitly]
        private void Update()
        {
            long elapsedTime = gameTimer.ElapsedMilliseconds;
            int worldTimeDiff = (int)(elapsedTime - lastWorldUpdateTime);
            int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);
            float gameTimeFloatDiff = gameTimeDiff / 1000.0f;

            lastGameUpdateTime = elapsedTime;
            switch (updatePolicy)
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

            var coreModules = worldSession.CoreModules;
            for (var i = 0; i < coreModules.Count; i++)
                coreModules[i].DoUpdate(gameTimeDiff);

            var clientModules = worldSession.ClientModules;
            for (var i = 0; i < clientModules.Count; i++)
                clientModules[i].DoUpdate(gameTimeFloatDiff);
        }
    }
}