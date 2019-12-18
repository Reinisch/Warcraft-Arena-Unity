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

        public Vector3 NextPosition { get => navmeshAgent.nextPosition; set => navmeshAgent.nextPosition = value; }
        public float Speed { get => navmeshAgent.speed; set => navmeshAgent.speed = value; }
        public float AngularSpeed { get => navmeshAgent.angularSpeed; set => navmeshAgent.angularSpeed = value; }
        public bool UpdateRotation { get => navmeshAgent.updateRotation; set => navmeshAgent.updateRotation = value; }
        public bool UpdatePosition { get => navmeshAgent.updatePosition; set => navmeshAgent.updatePosition = value; }
        public bool NavMeshAgentEnabled { get => navmeshAgent.enabled; set => navmeshAgent.enabled = value; }
        public bool HasPendingPath => navmeshAgent.pathPending;
        public bool HasPath => navmeshAgent.hasPath;
        public float RemainingDistance => navmeshAgent.remainingDistance;

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

        public bool SetDestination(Vector3 destination) => navmeshAgent.SetDestination(destination);

        public bool SetPath(NavMeshPath path) => navmeshAgent.SetPath(path);
    }
}
