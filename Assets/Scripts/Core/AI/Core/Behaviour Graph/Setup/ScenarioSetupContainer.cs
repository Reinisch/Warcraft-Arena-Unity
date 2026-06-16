using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.BehaviorGraph
{
    public class ScenarioSetupContainer: MonoBehaviour
    {
        [field: SerializeField]
        public List<ScenarioSetupAction> SetupActions {get; private set;} = new();

        [Inject]
        public Map Map {get; private set;}

#if UNITY_EDITOR
        [UsedImplicitly, ContextMenu("Collect scenario actions")]
        private void CollectScenario()
        {
            SetupActions = new List<ScenarioSetupAction>(GetComponentsInChildren<ScenarioSetupAction>());
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}