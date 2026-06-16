using System.Collections.Generic;
using Client.Spells;
using Common;
using UnityEngine;

namespace Client
{
    public sealed partial class UnitRenderer
    {
        private class AuraEffectController : IVisibleAuraHandler
        {
            private class SpellVisualAuraState
            {
                private EffectHandle Handle { get; }

                public SpellVisualAuraState(EffectHandle handle)
                {
                    Handle = handle;
                }

                public void Fade()
                {
                    Handle.Fade();
                }

                public void Stop()
                {
                    Handle.Stop();
                }

                public void Replay()
                {
                    Handle.Replay();
                }
            }

            private bool isDetaching;
            private UnitRenderer unitRenderer;
            private readonly Dictionary<int, List<IVisibleAura>> aurasByAuraId = new Dictionary<int, List<IVisibleAura>>();
            private readonly Dictionary<int, SpellVisualAuraState> effectByAuraId = new Dictionary<int, SpellVisualAuraState>();
            private readonly HashSet<IVisibleAura> aurasPreventingAnimation = new HashSet<IVisibleAura>();

            public void HandleAttach(UnitRenderer unitRenderer)
            {
                this.unitRenderer = unitRenderer;
                unitRenderer.Unit.FindBehaviour<AuraControllerClient>().AddHandler(this);
            }

            public void HandleDetach()
            {
                isDetaching = true;
                unitRenderer.Unit.FindBehaviour<AuraControllerClient>().RemoveHandler(this);
                unitRenderer = null;

                Assert.IsTrue(aurasByAuraId.Count == 0);
                Assert.IsTrue(effectByAuraId.Count == 0);
                Assert.IsTrue(aurasPreventingAnimation.Count == 0);
                isDetaching = false;
            }

            public void AuraApplied(IVisibleAura visibleAura)
            {
                aurasByAuraId.Insert(visibleAura.AuraId, visibleAura);

                if (effectByAuraId.ContainsKey(visibleAura.AuraId))
                    return;

                if (!unitRenderer.rendering.AuraVisuals.TryGetValue(visibleAura.AuraId, out AuraVisualsInfo settings))
                    return;

                if (settings.PreventAnimation)
                    HandleAnimationPreventingAuras(visibleAura, true);

                if (settings.EffectSettings == null)
                    return;

                Vector3 effectDirection = Vector3.ProjectOnPlane(unitRenderer.transform.forward, Vector3.up);
                Quaternion effectRotation = Quaternion.LookRotation(effectDirection);
                EffectHandle handle = settings.EffectSettings.PlayEffect(unitRenderer.transform.position, effectRotation);
                if (handle.IsValid)
                {
                    unitRenderer.TagContainer.ApplyPositioning(handle.Entity, settings);
                    effectByAuraId[visibleAura.AuraId] = new SpellVisualAuraState(handle);
                }
            }

            public void AuraUnapplied(IVisibleAura visibleAura)
            {
                aurasByAuraId.Delete(visibleAura.AuraId, visibleAura);
                HandleAnimationPreventingAuras(visibleAura, false);

                if (aurasByAuraId.ContainsKey(visibleAura.AuraId) || !effectByAuraId.TryGetValue(visibleAura.AuraId, out SpellVisualAuraState visualToRemove))
                    return;

                if (isDetaching)
                    visualToRemove.Stop();
                else
                    visualToRemove.Fade();

                effectByAuraId.Remove(visibleAura.AuraId);
            }

            public void AuraRefreshed(IVisibleAura visibleAura)
            {
                if (effectByAuraId.TryGetValue(visibleAura.AuraId, out SpellVisualAuraState activeState))
                    activeState.Replay();
            }

            private void HandleAnimationPreventingAuras(IVisibleAura aura, bool applied)
            {
                bool wasAnimated = aurasPreventingAnimation.Count == 0;

                if (applied)
                    aurasPreventingAnimation.Add(aura);
                else
                    aurasPreventingAnimation.Remove(aura);

                bool canAnimate = aurasPreventingAnimation.Count == 0;
                if (wasAnimated != canAnimate)
                    unitRenderer.UpdateAnimationState(canAnimate);
            }
        }
    }
}