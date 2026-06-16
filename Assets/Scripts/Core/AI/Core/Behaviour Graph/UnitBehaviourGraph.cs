using Common;
using Unity.Behavior;
using UnityEngine;

namespace Core
{
    public sealed class UnitBehaviourGraph : IUnitAIModel
    {
        public const string BlackboardUnitVariableName = "Unit";
        public const string BlackboardUnitAIVariableName = "Unit AI";

        private UnitBehaviourGraphSettings GraphSettings { get; }

        private BehaviorGraphAgent agent;

        public Unit Unit => UnitAI.Unit;
        public UnitAI UnitAI { get; private set; }

        public UnitBehaviourGraph(UnitBehaviourGraphSettings graphSettings)
        {
            GraphSettings = graphSettings;
        }

        void IUnitAIModel.Register(UnitAI unitAI)
        {
            UnitAI = unitAI;

            agent = GameObjectPool.Take(
                GraphSettings.GraphPrefab,
                 Unit.transform.position,
                  Unit.transform.rotation,
                   Unit.transform)
                .GetComponent<BehaviorGraphAgent>();

            agent.SetVariableValue(BlackboardUnitVariableName, Unit);
            agent.SetVariableValue(BlackboardUnitAIVariableName, UnitAI);

            agent.Init();
            agent.Start();
        }

        void IUnitAIModel.Unregister()
        {
            agent.End();
            GameObjectPool.Return(agent.gameObject, false);
            agent = null;
            UnitAI = null;
        }

        // Unity Behavior graphs tick automatically through the agent's MonoBehaviour
        // Update loop, so no manual per-frame propagation is required here.
        void IUnitAIModel.DoUpdate(int deltaTime) { }
    }
}
