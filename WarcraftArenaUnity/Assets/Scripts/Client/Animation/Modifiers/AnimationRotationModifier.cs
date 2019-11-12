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
        [SerializeField, UsedImplicitly] private string parameterTarget;

        private int parameterHash;

        [UsedImplicitly]
        private void Awake() => parameterHash = Animator.StringToHash(parameterTarget);

        [UsedImplicitly]
        private void LateUpdate()
        {
            float currentValue = targetAnimator.GetFloat(parameterHash);
            float rotationValue = Mathf.InverseLerp(parameterRange.x, parameterRange.y, currentValue);
            Vector3 finalEulerRotation = Vector3.Lerp(-maxRotation, maxRotation, rotationValue);
            targetBone.localRotation *= Quaternion.Euler(finalEulerRotation);
        }
    }
}
