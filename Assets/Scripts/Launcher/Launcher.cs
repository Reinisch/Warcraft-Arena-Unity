using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class Launcher: MonoBehaviour
    {
        [SerializeField]
        private string bossFightLevel;

#if UNITY_EDITOR
        [SerializeField]
        [Tooltip("Editor only: Warmup graphs to avoid 'Aborting graph tick' auto pause on the first real tick.")]
        private List<BehaviorGraph> editorWarmupGraphs = new();
#endif

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
                foreach (BehaviorGraph graphAsset in editorWarmupGraphs)
                {
                    if (graphAsset == null)
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
            finally
            {
                Debug.unityLogger.logEnabled = previousLogEnabled;
                UnityEditor.EditorApplication.isPaused = false;
            }
#endif
        }
    }
}
