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
        [SerializeField, UsedImplicitly] private bool shouldApplyEvenBackward;
        [SerializeField, UsedImplicitly] private bool shouldApplyOnlyWhenAttacking;
        [SerializeField, UsedImplicitly] private bool revertWhenBackward;

        private int strafeHash;
        private int attackHash;
        private int forwardHash;

        private float attackValue;
        private float forwardValue;
        private float rotationValue = 0.5f;

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
            if (!targetAnimator.enabled)
                return;

            float currentValue = targetAnimator.GetFloat(strafeHash);
            bool isAttacking = targetAnimator.GetBool(attackHash);
            forwardValue = targetAnimator.GetFloat(forwardHash);
            attackValue = Mathf.MoveTowards(attackValue, isAttacking ? 1.0f : 0.0f, 10 * Time.deltaTime);

            float newRotationValue;

            if (forwardValue < 0.0f && !shouldApplyEvenBackward)
                newRotationValue = 0.5f;
            else if (attackValue <= 0.5f && shouldApplyOnlyWhenAttacking)
                newRotationValue = 0.5f;
            else
                newRotationValue = Mathf.InverseLerp(parameterRange.x, parameterRange.y, currentValue);

            if (forwardValue < 0.0f && revertWhenBackward)
                newRotationValue = 1.0f - newRotationValue;

            rotationValue = Mathf.MoveTowards(rotationValue, newRotationValue, 2 * Time.deltaTime);

            Vector3 finalEulerRotation = Vector3.Lerp(-maxRotation, maxRotation, rotationValue) * (1 - Mathf.Abs(forwardValue) / 2) * (1 - attackValue / 3);
            targetBone.localRotation = overrideRotation ? Quaternion.Euler(finalEulerRotation)
                : targetBone.localRotation * Quaternion.Euler(finalEulerRotation);
        }
    }
}
