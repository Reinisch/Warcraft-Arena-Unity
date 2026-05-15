using UnityEngine;

namespace Core.Scenario
{
    public abstract class ScenarioAction: MonoBehaviour
    {
        private MapScenario mapScenario;

        protected Map Map => mapScenario.Map;
        protected World World => Map.World;
        protected BalanceReference Balance => Map.Settings.Balance;

        internal virtual void Initialize(MapScenario scenario)
        {
            mapScenario = scenario;
        }

        internal virtual void DeInitialize()
        {
            mapScenario = null;
        }

        internal virtual void DoUpdate(int deltaTime)
        {
        }
    }
}
