using UnityEngine;

namespace Core
{
    public abstract class UnitBehaviour : MonoBehaviour, IUnitBehaviour
    {
        protected Unit Unit { get; private set; }

        public abstract bool HasClientLogic { get; }
        public abstract bool HasServerLogic { get; }

        void IUnitBehaviour.DoUpdate(int deltaTime)
        {
            OnUpdate(deltaTime);
        }

        void IUnitBehaviour.HandleUnitAttach(Unit unit)
        {
            Unit = unit;

            OnAttach();
        }

        void IUnitBehaviour.HandleUnitDetach()
        {
            OnDetach();

            Unit = null;
        }

        protected virtual void OnUpdate(int deltaTime)
        {
        }

        protected virtual void OnAttach()
        {
        }

        protected virtual void OnDetach()
        {
        }
    }
}
