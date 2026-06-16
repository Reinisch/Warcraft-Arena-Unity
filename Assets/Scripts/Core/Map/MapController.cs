using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core
{
    public class MapController: IInitializable
    {
        private class MapSlot
        {
            public Map Map { get; private set;}
            public Rect Rect { get; }

            public MapSlot(Rect rect)
            {
                Rect = rect;
            }

            public void AssignMap(Map map)
            {
                Map = map;
            }
        }

        private readonly struct LoadRequest
        {
            public readonly string MapName;
            public readonly Vector3 Offset;
            public readonly UniTaskCompletionSource<Scene> Completion;

            public LoadRequest(string mapName, Vector3 offset, UniTaskCompletionSource<Scene> completion)
            {
                MapName = mapName;
                Offset = offset;
                Completion = completion;
            }
        }

        [Inject]
        private World world;

        [Inject]
        private ZenjectSceneLoader sceneLoader;

        private readonly List<Map> loadedMaps = new();
        private readonly List<MapSlot> occupiedSlots = new();
        private readonly Dictionary<Map, Scene> mapScenes = new();
        private readonly Queue<LoadRequest> loadQueue = new();
        private readonly CancellationTokenSource disposeCts = new();

        private float slotSize = 1.0f;

        public IReadOnlyCollection<Map> LoadedMaps => loadedMaps;
        public Map PrimaryMap => loadedMaps.Count > 0 ? loadedMaps[0] : null;

        /// <summary>Raised after a map finishes loading. Used by the net layer to materialise buffered shadows.</summary>
        public event Action<Map> EventMapLoaded;

        /// <summary>Raised before a map is unloaded. Used by the net layer to signal clients the map is over.</summary>
        public event Action<Map> EventMapUnloaded;

        void IInitializable.Initialize()
        {
            ProcessLoadQueueAsync().Forget();
        }

        internal void Dispose()
        {
            disposeCts.Cancel();
            disposeCts.Dispose();
        }

        /// <param name="runScenario">
        /// When true (server/host) the scenario graph runs — spawning creatures, driving boss logic.
        /// A remote client passes false: it loads the scene only (grid + navmesh) and lets units arrive
        /// via replication, so no server logic runs locally.
        /// </param>
        public async UniTask<Map> LoadMapAsync(ScenarioDefinition scenario, bool unloadOthers = false, bool runScenario = true)
        {
            if (unloadOthers)
                await UnloadAllAsync();

            (Vector3, Rect) emptySlotOffset = FindEmptySlot(scenario.Map.FootprintSize);
            MapSlot mapSlot = new(emptySlotOffset.Item2);
            occupiedSlots.Add(mapSlot);

            var completion = new UniTaskCompletionSource<Scene>();
            loadQueue.Enqueue(new LoadRequest(scenario.Map.MapName, emptySlotOffset.Item1, completion));
            Scene uniqueSceneInstance = await completion.Task;

            var registry = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>();
            SceneContext sceneContext = registry.GetSceneContextForScene(uniqueSceneInstance);
            Map map =  sceneContext.Container.Resolve<Map>();
            Scene mapScene = map.Settings.gameObject.scene;

            mapSlot.AssignMap(map);
            loadedMaps.Add(map);
            mapScenes[map] = mapScene;
           
            map.RelocateGrid();
            map.Settings.RegisterNavMeshes();

            await UniTask.Yield();
            SceneManager.SetActiveScene(mapScene);

            if (runScenario)
                map.SetScenario(scenario);

            EventMapLoaded?.Invoke(map);

            return map;
        }

        public async UniTask UnloadMapAsync(Map map)
        {
            if (!loadedMaps.Contains(map))
                return;

            EventMapUnloaded?.Invoke(map);

            world.UnitManager.DestroyMapUnits(map);
            map.Dispose();

            loadedMaps.Remove(map);
            occupiedSlots.RemoveAll(slot => slot.Map == map);

            if (mapScenes.TryGetValue(map, out Scene mapScene))
            {
                mapScenes.Remove(map);
                if (mapScene.IsValid())
                    await SceneManager.UnloadSceneAsync(mapScene);
            }
        }

        public async UniTask UnloadAllAsync()
        {
            foreach (Map map in new List<Map>(loadedMaps))
                await UnloadMapAsync(map);
        }
       
        private async UniTaskVoid ProcessLoadQueueAsync()
        {
            CancellationToken cancellationToken = disposeCts.Token;

            try
            {
                while (true)
                {
                    await UniTask.WaitUntil(() => loadQueue.Count > 0, cancellationToken: cancellationToken);

                    LoadRequest request = loadQueue.Dequeue();
                    int sceneCountBeforeLoad = SceneManager.sceneCount;
                    AsyncOperation loadOperation = sceneLoader.LoadSceneAsync(request.MapName, LoadSceneMode.Additive);

                    loadOperation.allowSceneActivation = false;
                    while (!loadOperation.isDone)
                    {
                        if (!loadOperation.allowSceneActivation && loadOperation.progress >= 0.9f)
                            loadOperation.allowSceneActivation = true;

                        await UniTask.Yield(cancellationToken);
                    }

                    Scene scene = SceneManager.GetSceneAt(sceneCountBeforeLoad);

                    foreach (GameObject rootObj in scene.GetRootGameObjects())
                        rootObj.transform.position += request.Offset;

                    request.Completion.TrySetResult(scene);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private (Vector3, Rect) FindEmptySlot(Vector2 footprint)
        {
            slotSize = Mathf.Max(slotSize, footprint.x, footprint.y);

            for (int ring = 0; ; ring++)
            {
                foreach (Vector2Int cell in EnumerateRing(ring))
                {
                    Vector2 origin = new(cell.x * slotSize, cell.y * slotSize);
                    Rect rect = new(origin, footprint);

                    bool overlaps = false;
                    foreach (var slot in occupiedSlots)
                    {
                        if (slot.Rect.Overlaps(rect))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        return (new Vector3(origin.x, 0.0f, origin.y), rect);
                    }
                }
            }
        }

        private static IEnumerable<Vector2Int> EnumerateRing(int ring)
        {
            if (ring == 0)
            {
                yield return Vector2Int.zero;
                yield break;
            }

            for (int x = -ring; x <= ring; x++)
            for (int y = -ring; y <= ring; y++)
                if (Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) == ring)
                    yield return new Vector2Int(x, y);
        }
    }
}
