using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.AI.Navigation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace Core
{
    public class MapSettings : MonoBehaviour
    {
        [SerializeField, UsedImplicitly, Range(2.0f, 50.0f)] private float gridCellSize;
        [SerializeField, UsedImplicitly] private Transform defaultSpawnPoint;
        [SerializeField, UsedImplicitly] private BoxCollider boundingBox;
        [SerializeField, UsedImplicitly] private MapDefinition mapDefinition;
        [SerializeField, UsedImplicitly] private MapScenarioGraphSettings scenarioSettings;
        [SerializeField, UsedImplicitly] private List<NavMeshSurface> navMeshSurfaces = new();

        internal float GridCellSize => gridCellSize;
        internal BoxCollider BoundingBox => boundingBox;
        internal Transform DefaultSpawnPoint => defaultSpawnPoint;
        internal MapDefinition Definition => mapDefinition;
        internal MapScenarioGraphSettings ScenarioSettings => scenarioSettings;

        /// <summary>
        /// Restores the navmesh data reference on each tracked surface (which the Editor clears
        /// when the surface is duplicated via additive scene loading) and re-registers it at the
        /// surface's current position, without rebuilding the navmesh at runtime.
        /// </summary>
        internal void RegisterNavMeshes()
        {
            foreach (NavMeshSurface surface in navMeshSurfaces)
                surface.BuildNavMesh();
        }

#if UNITY_EDITOR
        [InitializeOnLoad]
        private static class AutoCollector
        {
            static AutoCollector()
            {
                EditorSceneManager.sceneSaving += OnSceneSaving;
            }

            private static void OnSceneSaving(Scene scene, string path)
            {
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    foreach (MapSettings settings in root.GetComponentsInChildren<MapSettings>(true))
                        settings.CollectNavMeshSurfaces();
                }
            }
        }

        [ContextMenu("Collect NavMesh Surfaces"), UsedImplicitly]
        private void CollectNavMeshSurfaces()
        {
            var collected = new List<NavMeshSurface>();
            foreach (GameObject root in gameObject.scene.GetRootGameObjects())
                    collected.AddRange(root.GetComponentsInChildren<NavMeshSurface>(true));

            bool unchanged = navMeshSurfaces.Count == collected.Count;
            if (unchanged)
            {
                for (int i = 0; i < collected.Count; i++)
                {
                    if (navMeshSurfaces[i] != collected[i])
                    {
                        unchanged = false;
                        break;
                    }
                }
            }

            if (unchanged)
                return;

            navMeshSurfaces = collected;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
