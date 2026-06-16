using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public class EffectBehaviourFaceCamera : EffectBehaviour
    {
        [Inject] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private Transform transformToRotate;
        [SerializeField, UsedImplicitly] private Vector3 rotationOffset;

        protected override void OnUpdate(IEffectEntity effectEntity, float deltaTime, ref bool keepAlive)
        {
            base.OnUpdate(effectEntity, deltaTime,ref keepAlive);

            if (cameraReference.WarcraftCamera != null)
            {
                Quaternion lookDirectionOffset = Quaternion.Euler(rotationOffset);
                Vector3 projectedCameraDirection = Vector3.ProjectOnPlane(cameraReference.WarcraftCamera.transform.forward, Vector3.up);
                transformToRotate.rotation = Quaternion.LookRotation(projectedCameraDirection) * lookDirectionOffset;
            }
        }
    }
}