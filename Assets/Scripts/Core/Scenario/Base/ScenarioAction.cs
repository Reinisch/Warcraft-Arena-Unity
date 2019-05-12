using UnityEngine;

namespace Core
{
    public abstract class ScenarioAction : MonoBehaviour
    {
        protected Map Map { get; private set; }

        public void Initialize(Map map)
        {
            Map = map;
        }

        public void DeInitialize()
        {
            Map = null;
        }

        public abstract void DoUpdate(int deltaTime);
    }
}
