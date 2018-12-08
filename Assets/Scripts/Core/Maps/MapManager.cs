using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Core
{
    public class MapManager
    {
        public static MapManager Instance { get; } = new MapManager();

        private static readonly Dictionary<GridStateType, GridState> GridStates = new Dictionary<GridStateType, GridState>(4);
        private static readonly Dictionary<int, Map> BaseMaps = new Dictionary<int, Map>();
        private static readonly Mutex MapsLock = new Mutex(true);
        private static readonly BitArray InstanceIds = new BitArray(255);
        private static readonly MapUpdater MapUpdater = new MapUpdater();

        private static int scheduledScripts;
        private static int gridCleanUpDelay;

        private int NextInstanceId { get; set; }

        public bool IsScriptScheduled => scheduledScripts > 0;

        private MapManager() { }

        public static void Initialize()
        {
            gridCleanUpDelay = 1000; // TODO: add config

            GridStates[GridStateType.Invalid] = new InvalidState();
            GridStates[GridStateType.Active] = new ActiveState();
            GridStates[GridStateType.Idle] = new IdleState();
            GridStates[GridStateType.Removal] = new RemovalState();

            int numThreads = 8;
            if (numThreads > 0)
                MapUpdater.Activate(numThreads);
        }

        public static void Deinitialize()
        {
            foreach (var mapEntry in BaseMaps)
            {
                mapEntry.Value.UnloadAll();
                mapEntry.Value.Deinitialize();
            }
            BaseMaps.Clear();

            if (MapUpdater.Activated)
                MapUpdater.Deactivate();

            GridStates.Clear();
        }

        public void DoUpdate(int timeDiff)
        {
            foreach (var map in BaseMaps)
            {
                if (MapUpdater.Activated)
                    MapUpdater.ScheduleUpdate(map.Value, timeDiff);
                else
                    map.Value.DoUpdate(timeDiff);
            }
        
            if (MapUpdater.Activated)
                MapUpdater.Wait();

            foreach (var mapEntry in BaseMaps)
                mapEntry.Value.DelayedUpdate(timeDiff);
        }


        public Map CreateBaseMap(int mapId)
        {
            Map map = BaseMaps.LookupEntry(mapId);

            if (map == null)
            {
                MapsLock.WaitOne();

                map = new Map(mapId, gridCleanUpDelay, 0);
                BaseMaps[mapId] = map;
                map.Initialize(SceneManager.GetActiveScene());

                MapsLock.ReleaseMutex();
            }

            Assert.IsNotNull(map);
            return map;
        }

        public Map FindMap(int mapId)
        {
            return BaseMaps.LookupEntry(mapId);
        }

        public int GenerateInstanceId()
        {
            int newInstanceId = NextInstanceId;

            for (int i = ++NextInstanceId; i < int.MaxValue; ++i)
            {
                if (i < InstanceIds.Length && !InstanceIds.Get(i) || i >= InstanceIds.Length)
                {
                    NextInstanceId = i;
                    break;
                }
            }

            if (newInstanceId == NextInstanceId)
            {
                Debug.LogError("Instance ID overflow! Can't continue, shutting down server.");
                Application.Quit();
            }

            // allocate space if necessary
            if (newInstanceId >= InstanceIds.Length)
                InstanceIds.Length = newInstanceId + 1;

            InstanceIds[newInstanceId] = true;

            return newInstanceId;
        }

        public void InitInstanceIds()
        {
            NextInstanceId = 1;

            int result = 255;
            if (result > 0)
                InstanceIds.Length = result;
        }

        public void RegisterInstanceId(int instanceId)
        {
            // allocation and sizing was done in InitInstanceIds()
            InstanceIds[instanceId] = true;
        }

        public void FreeInstanceId(int instanceId)
        {
            // if freed instance id is lower than the next id available for new instances, use the freed one instead
            if (instanceId < NextInstanceId)
                NextInstanceId = instanceId;

            InstanceIds[instanceId] = false;
        }

        public EnterState PlayerCannotEnter(uint mapId, Player player, bool loginCheck = false) { throw new NotImplementedException(); }


        public static bool IsValidMap(int mapId, bool startUp)
        {
            MapDefinition mapDefinition = BalanceManager.MapsById.LookupEntry(mapId);

            if (startUp)
                return mapDefinition != null;

            return mapDefinition != null && !mapDefinition.IsDungeon();
        }

        public void DoForAllMaps(Action<Map> mapAction)
        {
            MapsLock.WaitOne();

            foreach (var mapEntry in BaseMaps)
                mapAction(mapEntry.Value);
           
            MapsLock.ReleaseMutex();
        }

        public void DoForAllMapsWithMapId(int mapId, Action<Map> mapAction)
        {
            MapsLock.WaitOne();

            var map = BaseMaps.LookupEntry(mapId);
            if (map != null)
            {
                mapAction(map);
            }

            MapsLock.ReleaseMutex();
        }


        public void IncreaseScheduledScriptsCount() { Interlocked.Increment(ref scheduledScripts); }

        public void DecreaseScheduledScriptCount() { Interlocked.Decrement(ref scheduledScripts); }

        public void DecreaseScheduledScriptCount(int count) { Interlocked.Add(ref scheduledScripts, -count); }
    }
}