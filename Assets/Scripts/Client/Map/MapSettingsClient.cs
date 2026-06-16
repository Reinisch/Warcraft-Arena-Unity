using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace Client
{
    /// <summary>
    /// Tracks objects on a map that are only relevant for the local presentation of that map
    /// (e.g. lights, minimap), so they can be disabled while the map is loaded but not the active one.
    /// </summary>
    public class MapSettingsClient : MonoBehaviour
    {
        public const string GameplayIrrelevantTag = "MapLocalObject";

        [SerializeField, UsedImplicitly] private List<GameObject> gameplayIrrelevantObjects = new();

        public void SetActive(bool active)
        {
            foreach (GameObject gameObject in gameplayIrrelevantObjects)
            {
                if (gameObject != null)
                    gameObject.SetActive(active);
            }
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
                    foreach (MapSettingsClient settingsClient in root.GetComponentsInChildren<MapSettingsClient>(true))
                        settingsClient.CollectGameplayIrrelevantObjects();
                }
            }
        }

        [ContextMenu("Collect Gameplay Irrelevant Objects"), UsedImplicitly]
        private void CollectGameplayIrrelevantObjects()
        {
            var collected = new List<GameObject>();
            foreach (GameObject root in gameObject.scene.GetRootGameObjects())
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.CompareTag(GameplayIrrelevantTag))
                    collected.Add(transform.gameObject);
            }

            if (gameplayIrrelevantObjects.Count == collected.Count && new HashSet<GameObject>(gameplayIrrelevantObjects).SetEquals(collected))
                return;

            gameplayIrrelevantObjects = collected;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
