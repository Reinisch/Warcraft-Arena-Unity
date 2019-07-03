using Client.Spells;
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
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellLabel;
        [SerializeField, UsedImplicitly] private Image spellIcon;
        [SerializeField, UsedImplicitly] private Slider castSlider;

        private Unit caster;
        private bool isCasting;

        public void DoUpdate()
        {
            UpdateState();

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

            isCasting = false;

            UpdateState();
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

        private void UpdateState()
        {
            canvasGroup.blocksRaycasts = isCasting;
            canvasGroup.interactable = isCasting;
            canvasGroup.alpha = isCasting ? 1.0f : 0.0f;
        }

        private void OnSpellCastChanged()
        {
            isCasting = caster.EntityState.SpellCast.Id != 0;
            if (isCasting && balanceReference.SpellInfosById.TryGetValue(caster.EntityState.SpellCast.Id, out SpellInfo spellInfo))
                spellLabel.text = spellInfo.SpellName;
            else
                spellLabel.text = string.Empty;

            if (isCasting && rendering.SpellVisualSettingsById.TryGetValue(caster.EntityState.SpellCast.Id, out SpellVisualSettings settings))
                spellIcon.sprite = settings.SpellIcon;
            else
                spellIcon.sprite = rendering.DefaultSpellIcon;
        }
    }
}