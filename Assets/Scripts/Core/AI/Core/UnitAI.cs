using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Core
{
    public abstract class UnitAI : UnitBehaviour
    {
        [SerializeField, UsedImplicitly]
        private NavMeshAgent navmeshAgent;

        [Inject]
        private DiContainer diContainer;

        private IUnitAIModel unitAIModel;

        public override bool HasClientLogic => false;
        public override bool HasServerLogic => true;

        public Vector3 NextPosition { get => navmeshAgent.nextPosition; set => navmeshAgent.nextPosition = value; }
        // Steering direction toward the current path target (independent of updatePosition) — used by the
        // character controller to face AI-driven movement, since the KCC owns transform rotation.
        public Vector3 DesiredVelocity => navmeshAgent.desiredVelocity;
        public float Speed { get => navmeshAgent.speed; set => navmeshAgent.speed = value; }
        public float AngularSpeed { get => navmeshAgent.angularSpeed; set => navmeshAgent.angularSpeed = value; }
        public bool UpdateRotation { get => navmeshAgent.updateRotation; set => navmeshAgent.updateRotation = value; }
        public bool UpdatePosition { get => navmeshAgent.updatePosition; set => navmeshAgent.updatePosition = value; }
        public bool NavMeshAgentEnabled { get => navmeshAgent.enabled; set => navmeshAgent.enabled = value; }
        public bool HasPendingPath => navmeshAgent.pathPending;
        public bool HasPath => navmeshAgent.hasPath;
        public float RemainingDistance => navmeshAgent.remainingDistance;
        public DiContainer DiContainer => diContainer;

        protected override void OnAttach()
        {
            base.OnAttach();

            navmeshAgent.enabled = false;

            if (Unit.Balance.UnitInfoAIById.TryGetValue(Unit.CreationToken.OriginalAIInfoId, out UnitInfoAI unitInfoAI))
            {
                unitAIModel = unitInfoAI.CreateAI();
                unitAIModel.Register(this);
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
