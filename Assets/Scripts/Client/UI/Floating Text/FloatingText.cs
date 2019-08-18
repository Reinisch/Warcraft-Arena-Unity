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
        [SerializeField, UsedImplicitly] private FloatingTextSettings damageCritSettings;
        [SerializeField, UsedImplicitly] private FloatingTextSettings healingSettings;
        [SerializeField, UsedImplicitly] private FloatingTextSettings healingCritSettings;

        private float currentLifeTime;
        private float targetLifeTime;
        private FloatingTextSettings currentSettings;

        [UsedImplicitly]
        private void OnDestroy()
        {
            GameObjectPool.Return(this, true);
        }

        public void SetDamage(int damageAmount, bool isCrit) => SetText(isCrit ? damageCritSettings : damageSettings, damageAmount);

        public void SetHealing(int healingAmount, bool isCrit) => SetText(isCrit ? healingCritSettings : healingSettings, healingAmount);

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

            textMesh.alpha = currentSettings.AlphaOverTime.Evaluate(currentLifeTime);
            return currentLifeTime >= targetLifeTime;
        }

        private void SetText(FloatingTextSettings newSettings, int value)
        {
            currentSettings = newSettings;
            textMesh.text = value.ToString();
            textMesh.font = newSettings.FontAsset;
            textMesh.fontSize = currentSettings.FontSize;
            textMesh.color = currentSettings.FontColor;
            targetLifeTime = currentSettings.LifeTime;
            transform.localScale = Vector3.one;
            currentLifeTime = 0;

            WarcraftCamera warcraftCamera = cameraReference.WarcraftCamera;
            if (warcraftCamera != null)
            {
                Vector3 direction = transform.position - warcraftCamera.transform.position;
                float distance = Vector3.Dot(direction, warcraftCamera.transform.forward);
                transform.position += Random.insideUnitSphere * currentSettings.RandomOffset * currentSettings.RandomOffsetOverDistance.Evaluate(distance);
            }
            else
                transform.position += Random.insideUnitSphere * currentSettings.RandomOffset;
        }
    }
}
