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
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private LocalizationReference localization;
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

            int expectedCastFrames = (int) (caster.SpellCast.State.CastTime / BoltNetwork.FrameDeltaTime / 1000.0f);
            castSlider.value = (float) (BoltNetwork.ServerFrame - caster.SpellCast.State.ServerFrame) / expectedCastFrames;
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

            caster.AddCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);
        }

        private void DeinitializeCaster()
        {
            caster.RemoveCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);

            isCasting = false;
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
            isCasting = caster.SpellCast.State.Id != 0;
            if (isCasting && localization.TooltipInfoBySpellId.TryGetValue(caster.SpellCast.State.Id, out SpellTooltipInfo tooltipInfo))
                spellLabel.text = tooltipInfo.SpellNameString.Value;
            else
                spellLabel.text = string.Empty;

            if (isCasting && rendering.SpellVisuals.TryGetValue(caster.SpellCast.State.Id, out SpellVisualsInfo settings))
                spellIcon.sprite = settings.SpellIcon;
            else
                spellIcon.sprite = rendering.DefaultSpellIcon;
        }
    }
}