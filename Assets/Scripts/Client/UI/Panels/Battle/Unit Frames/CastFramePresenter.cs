using Client.Spells;
using Client.UI;
using Core;
using Zenject;

namespace Client
{
    public class CastFramePresenter : Presenter<CastFrame>
    {
        [Inject] private RenderingReference rendering;
        [Inject] private LocalizationReference localization;

        private Unit caster;
        private bool isCasting;

        public void SetCaster(Unit newCaster)
        {
            if (caster != null)
                DeinitializeCaster();

            if (newCaster != null)
                InitializeCaster(newCaster);

            isCasting = false;

            UpdateState();
        }

        public void Tick()
        {
            UpdateState();

            if (!isCasting)
                return;

            View.SetCastProgress(1 - (float)caster.SpellCast.CastTimeLeft / caster.SpellCast.CastTime);
        }

        private void InitializeCaster(Unit newCaster)
        {
            caster = newCaster;

            caster.SpellCast.EventSpellCastChanged += OnSpellCastChanged;
        }

        private void DeinitializeCaster()
        {
            caster.SpellCast.EventSpellCastChanged -= OnSpellCastChanged;

            isCasting = false;
            caster = null;
        }

        private void UpdateState()
        {
            View.SetVisible(isCasting);
        }

        private void OnSpellCastChanged()
        {
            isCasting = caster.SpellCast.IsCasting;

            if (isCasting && localization.TooltipInfoBySpellId.TryGetValue(caster.SpellCast.CastingSpellInfo.Id, out SpellTooltipInfo tooltipInfo))
                View.SetSpellLabel(tooltipInfo.SpellNameString.Value);
            else
                View.SetSpellLabel(string.Empty);

            if (isCasting && rendering.SpellVisuals.TryGetValue(caster.SpellCast.CastingSpellInfo.Id, out SpellVisualsInfo settings))
                View.SetSpellIcon(settings.SpellIcon);
            else
                View.SetSpellIcon(rendering.DefaultSpellIcon);
        }
    }
}
