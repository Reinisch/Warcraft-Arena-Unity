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
        private readonly Dictionary<GridStateType, GridState> gridStates = new Dictionary<GridStateType, GridState>(4);
        private readonly Dictionary<int, Map> baseMaps = new Dictionary<int, Map>();
        private readonly Mutex mapsLock = new Mutex(true);
        private readonly BitArray instanceIds = new BitArray(255);
        private readonly MapUpdater mapUpdater = new MapUpdater();

        private int gridCleanUpDelay;
        private WorldManager worldManager;

        private int NextInstanceId { get; set; }

        public MapManager(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            gridCleanUpDelay = 1000;

            gridStates[GridStateType.Invalid] = new InvalidState();
            gridStates[GridStateType.Active] = new ActiveState();
            gridStates[GridStateType.Idle] = new IdleState();
            gridStates[GridStateType.Removal] = new RemovalState();

            if (!worldManager.HasClientLogic)
                mapUpdater.Activate(8);
        }

        public void Dispose()
        {
            foreach (var mapEntry in baseMaps)
            {
                mapEntry.Value.UnloadAll();
                mapEntry.Value.Deinitialize();
            }
            baseMaps.Clear();

            if (!worldManager.HasClientLogic)
                mapUpdater.Deactivate();

            gridStates.Clear();

            worldManager = null;
        }

        public void DoUpdate(int timeDiff)
        {
            foreach (var map in baseMaps)
            {
                if (mapUpdater.Activated)
                    mapUpdater.ScheduleUpdate(map.Value, timeDiff);
                else
                    map.Value.DoUpdate(timeDiff);
            }
        
            if (mapUpdater.Activated)
                mapUpdater.Wait();

            foreach (var mapEntry in baseMaps)
                mapEntry.Value.DelayedUpdate(timeDiff);
        }

        public Map CreateBaseMap(int mapId)
        {
            Map map = baseMaps.LookupEntry(mapId);

            if (map == null)
            {
                mapsLock.WaitOne();

                map = new Map(mapId, gridCleanUpDelay, 0);
                baseMaps[mapId] = map;
                map.Initialize(SceneManager.GetActiveScene());

                mapsLock.ReleaseMutex();
            }

            Assert.IsNotNull(map);
            return map;
        }

        public Map FindMap(int mapId)
        {
            return baseMaps.LookupEntry(mapId);
        }

        public int GenerateInstanceId()
        {
            int newInstanceId = NextInstanceId;

            for (int i = ++NextInstanceId; i < int.MaxValue; ++i)
            {
                if (i < instanceIds.Length && !instanceIds.Get(i) || i >= instanceIds.Length)
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
            if (newInstanceId >= instanceIds.Length)
                instanceIds.Length = newInstanceId + 1;

            instanceIds[newInstanceId] = true;

            return newInstanceId;
        }

        public void InitInstanceIds()
        {
            NextInstanceId = 1;

            int result = 255;
            if (result > 0)
                instanceIds.Length = result;
        }

        public void RegisterInstanceId(int instanceId)
        {
            // allocation and sizing was done in InitInstanceIds()
            instanceIds[instanceId] = true;
        }

        public void FreeInstanceId(int instanceId)
        {
            // if freed instance id is lower than the next id available for new instances, use the freed one instead
            if (instanceId < NextInstanceId)
                NextInstanceId = instanceId;

            instanceIds[instanceId] = false;
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
            mapsLock.WaitOne();

            foreach (var mapEntry in baseMaps)
                mapAction(mapEntry.Value);
           
            mapsLock.ReleaseMutex();
        }

        public void DoForAllMapsWithMapId(int mapId, Action<Map> mapAction)
        {
            mapsLock.WaitOne();

            var map = baseMaps.LookupEntry(mapId);
            if (map != null)
            {
                mapAction(map);
            }

            mapsLock.ReleaseMutex();
        }
    }
}