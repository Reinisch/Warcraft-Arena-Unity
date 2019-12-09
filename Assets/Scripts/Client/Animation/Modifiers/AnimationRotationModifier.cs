using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class AnimationRotationModifier : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private Animator targetAnimator;
        [SerializeField, UsedImplicitly] private Transform targetBone;
        [SerializeField, UsedImplicitly] private Vector2 parameterRange;
        [SerializeField, UsedImplicitly] private Vector3 maxRotation;
        [SerializeField, UsedImplicitly] private string attackParam;
        [SerializeField, UsedImplicitly] private string forwardParam;
        [SerializeField, UsedImplicitly] private string strafeParam;
        [SerializeField, UsedImplicitly] private bool overrideRotation;

        private int strafeHash;
        private int attackHash;
        private int forwardHash;

        private float attackValue;
        private float forwardValue;

        [UsedImplicitly]
        private void Awake()
        {
            strafeHash = Animator.StringToHash(strafeParam);
            attackHash = Animator.StringToHash(attackParam);
            forwardHash = Animator.StringToHash(forwardParam);
        }

        [UsedImplicitly]
        private void LateUpdate()
        {
            float currentValue = targetAnimator.GetFloat(strafeHash);
            bool isAttacking = targetAnimator.GetBool(attackHash);
            forwardValue = Mathf.MoveTowards(forwardValue, targetAnimator.GetFloat(forwardHash), 5 * Time.deltaTime);
            attackValue = Mathf.MoveTowards(attackValue, isAttacking ? 1.0f : 0.0f, 10 * Time.deltaTime);

            float rotationValue = Mathf.InverseLerp(parameterRange.x, parameterRange.y, currentValue);
            Vector3 finalEulerRotation = Vector3.Lerp(-maxRotation, maxRotation, rotationValue) * (1 - forwardValue / 2) * (1 - attackValue / 3);

            targetBone.localRotation = overrideRotation ? Quaternion.Euler(finalEulerRotation)
                : targetBone.localRotation * Quaternion.Euler(finalEulerRotation);
        }
    }
}
