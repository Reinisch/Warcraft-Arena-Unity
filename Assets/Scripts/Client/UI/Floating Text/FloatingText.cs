using Common;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Client
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TextMeshPro textMesh;
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private FloatingTextSettings damageSettings;

        private float currentLifeTime;
        private float targetLifeTime;
        private FloatingTextSettings currentSettings;

        [UsedImplicitly]
        private void OnDestroy()
        {
            GameObjectPool.Return(this, true);
        }

        public void SetDamage(int damageAmount)
        {
            textMesh.text = damageAmount.ToString();
            textMesh.fontSize = damageSettings.FontSize;
            targetLifeTime = damageSettings.LifeTime;
            currentSettings = damageSettings;
            transform.localScale = Vector3.one;
            currentLifeTime = 0;

            WarcraftCamera warcraftCamera = cameraReference.WarcraftCamera;
            if (warcraftCamera != null)
            {
                Vector3 direction = transform.position - warcraftCamera.transform.position;
                float distance = Vector3.Dot(direction, warcraftCamera.transform.forward);
                transform.position += Random.insideUnitSphere * damageSettings.RandomOffset * currentSettings.RandomOffsetOverDistance.Evaluate(distance);
            }
            else
                transform.position += Random.insideUnitSphere * damageSettings.RandomOffset;
        }

        public bool DoUpdate(float deltaTime)
        {
            currentLifeTime += deltaTime;

            transform.localScale = Vector3.one * currentSettings.SizeOverTime.Evaluate(currentLifeTime);
            transform.Translate(Vector3.up * currentSettings.FloatingSpeed * deltaTime);

            WarcraftCamera warcraftCamera = cameraReference.WarcraftCamera;
            if (warcraftCamera != null)
            {
                Vector3 direction = transform.position - warcraftCamera.transform.position;
                float distance = Vector3.Dot(direction, warcraftCamera.transform.forward);

                transform.rotation = Quaternion.LookRotation(warcraftCamera.transform.forward);
                transform.localScale *= currentSettings.SizeOverDistanceToCamera.Evaluate(distance);
            }

            return currentLifeTime >= targetLifeTime;
        }
    }
}
