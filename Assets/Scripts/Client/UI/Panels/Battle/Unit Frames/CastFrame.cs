using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class CastFrame : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private BalanceReference balanceReference;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellLabel;
        [SerializeField, UsedImplicitly] private Slider castSlider;

        private Unit caster;
        private bool isCasting;

        public void DoUpdate()
        {
            gameObject.SetActive(isCasting);
            if (!isCasting)
                return;

            int expectedCastFrames = (int) (caster.EntityState.SpellCast.CastTime / BoltNetwork.FrameDeltaTime / 1000.0f);
            castSlider.value = (float) (BoltNetwork.ServerFrame - caster.EntityState.SpellCast.ServerFrame) / expectedCastFrames;
        }

        public void UpdateCaster(Unit newCaster)
        {
            if (caster != null)
                DeinitializeCaster();

            if (newCaster != null)
                InitializeCaster(newCaster);

            gameObject.SetActive(caster != null && caster.EntityState.SpellCast.Id != 0);
        }

        private void InitializeCaster(Unit caster)
        {
            this.caster = caster;

            caster.EntityState.AddCallback(nameof(caster.EntityState.SpellCast), OnSpellCastChanged);
        }

        private void DeinitializeCaster()
        {
            isCasting = false;
            caster.EntityState.RemoveCallback(nameof(caster.EntityState.SpellCast), OnSpellCastChanged);
            caster = null;
        }

        private void OnSpellCastChanged()
        {
            isCasting = caster.EntityState.SpellCast.Id != 0;
            if (isCasting && balanceReference.SpellInfosById.TryGetValue(caster.EntityState.SpellCast.Id, out SpellInfo spellInfo))
                spellLabel.text = spellInfo.SpellName;
            else
                spellLabel.text = string.Empty;
        }
    }
}