using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class EffectBehaviourTimed : EffectBehaviour
    {
        [SerializeField, UsedImplicitly] private float waitTime = 1.0f;

        private float waitTimeLeft;

        protected override void OnPlay()
        {
            base.OnPlay();

            waitTimeLeft = waitTime;
        }

        protected override void OnUpdate(IEffectEntity effectEntity, float deltaTime, ref bool keepAlive)
        {
            base.OnUpdate(effectEntity, deltaTime, ref keepAlive);

            if (waitTimeLeft > 0.0f && (waitTimeLeft -= Time.deltaTime) > 0.0f)
                keepAlive = true;
        }
    }
}
