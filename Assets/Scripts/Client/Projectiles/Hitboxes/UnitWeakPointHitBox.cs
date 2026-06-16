using Client;
using UnityEngine;

namespace Core
{
    public class UnitWeakPointHitBox : MonoBehaviour
    {
        [field: SerializeField]
        public UnitProjectileHitBox HitBox { get; private set; }

        [SerializeField]
        private Transform weakPointParent;

        [SerializeField]
        private EffectSettings weakPointEffect;

        [SerializeField]
        private EffectSettings weakPointCompleteEffect;

        private EffectHandle weakPointEffectHandle;

        public int DamageLimit { get; private set; }

        private void Awake()
        {
            HitBox.EventDamageReceived += OnReceivedDamage;
            HitBox.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            HitBox.EventDamageReceived -= OnReceivedDamage;
        }

        public void Activate(int damageLimit, float damageMulti)
        {
            DamageLimit = damageLimit;
            HitBox.DamageMultiplier = damageMulti;

            weakPointEffectHandle.Stop();
            weakPointEffectHandle = weakPointEffect.PlayEffect(transform.position, transform.rotation, weakPointParent);
            weakPointEffectHandle.ResetLocally();

            HitBox.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            DamageLimit = 0;
            weakPointEffectHandle.Stop();
            weakPointCompleteEffect.PlayEffect(weakPointParent.position, weakPointParent.rotation);
            HitBox.gameObject.SetActive(false);
        }

        private void OnReceivedDamage(int damage)
        {
            if (!enabled)
                return;

            DamageLimit -= damage;

            if (DamageLimit <= 0)
                Deactivate();
        }
    }
}