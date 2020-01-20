using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(Camera))]
    public class MinimapCamera : MonoBehaviour
    {
        public Unit Target { get; set; }

        [UsedImplicitly]
        private void LateUpdate()
        {
            if (Target == null)
                return;

            transform.rotation = Quaternion.Euler(90.0f, Target.Rotation.eulerAngles.y, 0.0f);
            transform.position = new Vector3(Target.Position.x, transform.position.y, Target.Position.z);
        }
    }
}
