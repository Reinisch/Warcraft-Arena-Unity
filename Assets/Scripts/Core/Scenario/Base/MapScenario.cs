using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scenario
{
    public class MapScenario: MonoBehaviour
    {
        [SerializeField]
        private List<ScenarioAction> initialActions;

        internal Map Map { get; private set; }
        internal World World => Map.World;
        internal BalanceReference Balance => Map.Settings.Balance;

        internal void Initialize(Map map)
        {
            Map = map;

            initialActions.ForEach(item => item.Initialize(this));
        }

        internal void DeInitialize()
        {
            initialActions.ForEach(item => item.DeInitialize());
            Map = null;
        }

        internal virtual void DoUpdate(int deltaTime)
        {
            initialActions.ForEach(item => item.DoUpdate(deltaTime));
        }

#if UNITY_EDITOR
        [UsedImplicitly, ContextMenu("Collect scenario actions")]
        private void CollectScenario()
        {
            initialActions = new List<ScenarioAction>(GetComponentsInChildren<ScenarioAction>());
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}