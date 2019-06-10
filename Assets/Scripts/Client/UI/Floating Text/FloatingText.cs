using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TextMesh textMesh;
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
            transform.position += Random.insideUnitSphere * damageSettings.RandomOffset;
            targetLifeTime = damageSettings.LifeTime;
            currentSettings = damageSettings;
            transform.localScale = Vector3.one;
            currentLifeTime = 0;
        }

        public bool DoUpdate(float deltaTime)
        {
            currentLifeTime += deltaTime;

            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            transform.localScale = Vector3.one * currentSettings.SizeOverTime.Evaluate(currentLifeTime);
            transform.Translate(Vector3.up * currentSettings.FloatingSpeed * deltaTime);
            return currentLifeTime >= targetLifeTime;
        }
    }
}
