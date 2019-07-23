using System.Collections.Generic;
using Client.Spells;
using Common;

namespace Client
{
    public sealed partial class UnitRenderer
    {
        private class AuraEffectController : IVisibleAuraHandler
        {
            private class SpellVisualAuraState
            {
                private IEffectEntity EffectEntity { get; }
                private long PlayId { get; }

                public SpellVisualAuraState(long playId, IEffectEntity effectEntity)
                {
                    PlayId = playId;
                    EffectEntity = effectEntity;
                }

                public void Stop()
                {
                    EffectEntity.Stop(PlayId);
                }

                public void Replay()
                {
                    EffectEntity.Replay(PlayId);
                }
            }

            private UnitRenderer unitRenderer;
            private readonly Dictionary<int, List<IVisibleAura>> aurasByAuraId = new Dictionary<int, List<IVisibleAura>>();
            private readonly Dictionary<int, SpellVisualAuraState> effectByAuraId = new Dictionary<int, SpellVisualAuraState>();

            public void HandleAttach(UnitRenderer unitRenderer)
            {
                this.unitRenderer = unitRenderer;
                unitRenderer.Unit.FindBehaviour<AuraControllerClient>().AddHandler(this);
            }

            public void HandleDetach()
            {
                unitRenderer.Unit.FindBehaviour<AuraControllerClient>().RemoveHandler(this);
                unitRenderer = null;

                Assert.IsTrue(aurasByAuraId.Count == 0);
                Assert.IsTrue(effectByAuraId.Count == 0);
            }

            public void AuraApplied(IVisibleAura visibleAura)
            {
                aurasByAuraId.Insert(visibleAura.AuraId, visibleAura);

                if (effectByAuraId.ContainsKey(visibleAura.AuraId))
                    return;

                if (!unitRenderer.rendering.AuraVisualSettingsById.TryGetValue(visibleAura.AuraId, out AuraVisualSettings auraVisualSettings))
                    return;

                if (auraVisualSettings.EffectSettings == null)
                    return;

                IEffectEntity newEffect = auraVisualSettings.EffectSettings.PlayEffect(unitRenderer.transform.position, unitRenderer.transform.rotation, out long playId);
                if (newEffect != null)
                {
                    unitRenderer.TagContainer.ApplyPositioning(newEffect, auraVisualSettings);
                    effectByAuraId[visibleAura.AuraId] = new SpellVisualAuraState(playId, newEffect);
                }
            }

            public void AuraUnapplied(IVisibleAura visibleAura)
            {
                aurasByAuraId.Delete(visibleAura.AuraId, visibleAura);

                if (aurasByAuraId.ContainsKey(visibleAura.AuraId) || !effectByAuraId.TryGetValue(visibleAura.AuraId, out SpellVisualAuraState visualToRemove))
                    return;

                visualToRemove.Stop();
                effectByAuraId.Remove(visibleAura.AuraId);
            }

            public void AuraRefreshed(IVisibleAura visibleAura)
            {
                if (effectByAuraId.TryGetValue(visibleAura.AuraId, out SpellVisualAuraState activeState))
                    activeState.Replay();
            }
        }
    }
}