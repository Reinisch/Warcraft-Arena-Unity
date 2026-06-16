using Common;
using JetBrains.Annotations;
using System;
using Unity.Behavior;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    [Serializable]
    public abstract class BehaviourGraphActionWithUtility : BehaviourGraphAction
    {
        [SerializeField, UsedImplicitly]
        private BlackboardVariable<float> minUtilityInitial = new(2);

        [SerializeField, UsedImplicitly]
        private BlackboardVariable<float> maxUtilityInitial = new(3);

        [SerializeField, UsedImplicitly]
        private BlackboardVariable<float> minUtilityPerUse = new(1.5f);

        [SerializeField, UsedImplicitly]
        private BlackboardVariable<float> maxUtilityPerUse = new(2);

        private float utilityValue;

        protected override void OnSetup()
        {
            base.OnSetup();

            utilityValue = RandomUtils.Next(minUtilityInitial, maxUtilityInitial);
        }

        protected override void OnEnd()
        {
            utilityValue -= RandomUtils.Next(minUtilityPerUse, maxUtilityPerUse);

            base.OnEnd();
        }

        public virtual float GetUtility() => utilityValue;
    }
}