using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Launcher
{
    internal class Launcher: MonoBehaviour
    {
        [SerializeField]
        private string bossFightLevel;

        [Inject]
        private ZenjectSceneLoader sceneLoader;

        private void Awake()
        {
            ProjectContext.Instance.Container.Inject(this);

            WarmUpBehaviorGraphs();

            sceneLoader.LoadScene(bossFightLevel);
        }

        // Forces the JIT to compile the Update()/OnStart() methods of every node type used by these
        // graphs ahead of time, so the first real tick (which would otherwise hit the package's
        // 1 second "Aborting graph tick" watchdog after a domain reload) is already warm.
        private void WarmUpBehaviorGraphs()
        {
#if UNITY_EDITOR
            const int warmupTickCount = 5;

            bool previousLogEnabled = Debug.unityLogger.logEnabled;
            Debug.unityLogger.logEnabled = false;

            try
            {
                // The runtime BehaviorGraph is a sub-asset of each BehaviorAuthoringGraph (the
                // main .asset object), so search by the authoring type and pull the graph out.
                foreach (string guid in UnityEditor.AssetDatabase.FindAssets("t:BehaviorAuthoringGraph"))
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                    foreach (Object asset in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path))
                    {
                        if (asset is not BehaviorGraph graphAsset)
                            continue;

                        GameObject warmupObject = null;

                        try
                        {
                            warmupObject = new GameObject("Behaviour Graph Warmup (Editor Only)")
                            {
                                hideFlags = HideFlags.HideAndDontSave
                            };

                            var agent = warmupObject.AddComponent<BehaviorGraphAgent>();
                            agent.Graph = graphAsset;
                            agent.Init();
                            agent.Start();

                            for (int i = 0; i < warmupTickCount; i++)
                            {
                                agent.Graph.Tick();
                            }

                            agent.End();
                        }
                        catch
                        {
                            // Expected: the warmup agent has no real DI container, so
                            // nodes relying on those will throw. We only care about pre-JITting them.
                        }
                        finally
                        {
                            if (warmupObject != null)
                                DestroyImmediate(warmupObject);
                        }
                    }
                }
            }
            finally
            {
                Debug.unityLogger.logEnabled = previousLogEnabled;
                UnityEditor.EditorApplication.isPaused = false;
            }
#endif
        }
    }
}
