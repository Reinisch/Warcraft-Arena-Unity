using UnityEngine;

namespace Core.Scenario
{
    public abstract class ScenarioAction : MonoBehaviour
    {
        protected Map Map { get; private set; }
        protected World World => Map.World;
        protected BalanceReference Balance => Map.Settings.Balance;

        internal virtual void Initialize(Map map)
        {
            Map = map;
        }

        internal virtual void DeInitialize()
        {
            Map = null;
        }

        internal virtual void DoUpdate(int deltaTime)
        {
        }
    }
}
