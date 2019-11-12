using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    public abstract class UnitAI : UnitBehaviour
    {
        [SerializeField, UsedImplicitly] private NavMeshAgent navmeshAgent;
        [SerializeField, UsedImplicitly] private BalanceReference balance;

        private IUnitAIModel unitAIModel;

        public override bool HasClientLogic => false;
        public override bool HasServerLogic => true;

        protected override void OnAttach()
        {
            base.OnAttach();

            navmeshAgent.enabled = false;

            if (balance.UnitInfoAIById.TryGetValue(Unit.UnitCreateToken.OriginalAIInfoId, out UnitInfoAI unitInfoAI))
            {
                unitAIModel = unitInfoAI.CreateAI();
                unitAIModel.Register(Unit);
            }
        }

        protected override void OnDetach()
        {
            unitAIModel?.Unregister();
            unitAIModel = null;

            navmeshAgent.enabled = false;

            base.OnDetach();
        }

        protected override void OnUpdate(int deltaTime)
        {
            base.OnUpdate(deltaTime);

            unitAIModel?.DoUpdate(deltaTime);
        }
    }
}
