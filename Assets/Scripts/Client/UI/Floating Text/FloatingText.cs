using Client.Localization;
using Common;
using Core;
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
        [SerializeField, UsedImplicitly] private FloatingTextSettings fullAbsorbSettings;
        [SerializeField, UsedImplicitly] private FloatingTextSettings missSettings;
        [SerializeField, UsedImplicitly] private FloatingTextSettings healingSettings;
        [SerializeField, UsedImplicitly] private FloatingTextSettings healingCritSettings;
        [SerializeField, UsedImplicitly] private LocalizedString fullAbsrobString;

        private float currentLifeTime;
        private float targetLifeTime;
        private FloatingTextSettings currentSettings;

        [UsedImplicitly]
        private void OnDestroy()
        {
            GameObjectPool.Return(this, true);
        }

        public void SetMissText(SpellMissType missType)
        {
            SetText(missSettings, LocalizationReference.Localize(missType).Value);
        }

        public void SetDamage(int damageAmount, HitType hitType)
        {
            if (hitType == HitType.Immune)
                SetText(missSettings, LocalizationReference.Localize(SpellMissType.Immune).Value);
            else if (hitType.HasTargetFlag(HitType.FullAbsorb))
                SetText(fullAbsorbSettings, fullAbsrobString.Value);
            else
                SetText(hitType.HasTargetFlag(HitType.CriticalHit) ? damageCritSettings : damageSettings, damageAmount.ToString());
        }

        public void SetHealing(int healingAmount, bool isCrit)
        {
            SetText(isCrit ? healingCritSettings : healingSettings, healingAmount.ToString());
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

            textMesh.alpha = currentSettings.AlphaOverTime.Evaluate(currentLifeTime);
            return currentLifeTime >= targetLifeTime;
        }

        private void SetText(FloatingTextSettings newSettings, string value)
        {
            currentSettings = newSettings;
            textMesh.text = value;
            textMesh.fontSharedMaterial = newSettings.FontMaterial;
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
