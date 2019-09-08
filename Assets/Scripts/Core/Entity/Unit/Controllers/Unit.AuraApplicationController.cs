using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        internal class AuraApplicationController : IUnitBehaviour
        {
            private AuraInterruptFlags auraInterruptFlags;
            private Unit unit;

            private readonly Dictionary<AuraStateType, List<AuraApplication>> auraApplicationsByAuraState = new Dictionary<AuraStateType, List<AuraApplication>>();
            private readonly Dictionary<AuraEffectType, List<AuraEffect>> auraEffectsByAuraType = new Dictionary<AuraEffectType, List<AuraEffect>>();
            private readonly Dictionary<int, List<Aura>> ownedAurasById = new Dictionary<int, List<Aura>>();
            private readonly Dictionary<int, List<AuraApplication>> auraApplicationsByAuraId = new Dictionary<int, List<AuraApplication>>();
            private readonly List<AuraApplication> interruptableAuraApplications = new List<AuraApplication>();
            private readonly List<AuraApplication> auraApplications = new List<AuraApplication>();
            private readonly HashSet<AuraApplication> auraApplicationSet = new HashSet<AuraApplication>();
            private readonly List<Aura> ownedAuras = new List<Aura>();

            private readonly HashSet<AuraEffectHandleGroup> tempAuraHandleGroups = new HashSet<AuraEffectHandleGroup>();
            private readonly Dictionary<AuraApplication, AuraRemoveMode> tempApplicationsToRemove = new Dictionary<AuraApplication, AuraRemoveMode>();
            private readonly List<AuraApplication> tempAuraApplications = new List<AuraApplication>(10);
            private readonly List<int> tempAuraEffectsToHandle = new List<int>();

            internal IReadOnlyList<AuraApplication> AuraApplications => auraApplications;
            internal IReadOnlyList<Aura> OwnedAuras => ownedAuras;

            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            internal bool HasAuraType(AuraEffectType auraEffectType) => auraEffectsByAuraType.ContainsKey(auraEffectType);

            internal bool HasAuraState(AuraStateType auraStateType) => auraApplicationsByAuraState.ContainsKey(auraStateType);

            internal bool HasAuraAnyInterrupt(AuraInterruptFlags flag) => auraInterruptFlags.HasAnyFlag(flag);

            internal IReadOnlyList<AuraEffect> GetAuraEffects(AuraEffectType auraEffectType)
            {
                return auraEffectsByAuraType.TryGetValue(auraEffectType, out List<AuraEffect> effectList) ? effectList : null;
            }

            internal float TotalAuraModifier(AuraEffectType auraType)
            {
                if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                    return 0.0f;

                float modifier = 0.0f;

                foreach (AuraEffect auraEffect in auraEffects)
                    modifier += auraEffect.Value;

                return modifier;
            }

            internal float TotalAuraModifierForCaster(AuraEffectType auraType, ulong casterId)
            {
                if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                    return 0.0f;

                float modifier = 0.0f;

                foreach (AuraEffect auraEffect in auraEffects)
                    if (auraEffect.Aura.CasterId == casterId)
                        modifier += auraEffect.Value;

                return modifier;
            }

            internal float TotalAuraMultiplier(AuraEffectType auraType)
            {
                if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                    return 1.0f;

                float multiplier = 1.0f;

                foreach (AuraEffect auraEffect in auraEffects)
                    multiplier = multiplier.AddPercentage(auraEffect.Value);

                return multiplier;
            }

            internal float MaxPositiveAuraModifier(AuraEffectType auraType)
            {
                if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                    return 0.0f;

                float modifier = 0.0f;

                foreach (AuraEffect auraEffect in auraEffects)
                    modifier = Mathf.Max(modifier, auraEffect.Value);

                return modifier;
            }

            internal float MaxNegativeAuraModifier(AuraEffectType auraType)
            {
                if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                    return 0.0f;

                float modifier = 0.0f;

                foreach (AuraEffect auraEffect in auraEffects)
                    modifier = Mathf.Min(modifier, auraEffect.Value);

                return modifier;
            }

            internal void RefreshOrCreateAura(AuraInfo auraInfo, SpellInfo spellInfo, Unit originalCaster)
            {
                var ownedAura = FindOwnedAura();

                if (ownedAura != null && ownedAura.AuraInfo.HasAttribute(AuraAttributes.StackSameAuraInMultipleSlots))
                    ownedAura = null;

                if (ownedAura == null)
                {
                    ownedAura = new Aura(auraInfo, spellInfo, unit, originalCaster);
                    ownedAuras.Add(ownedAura);
                    ownedAurasById.Insert(ownedAura.AuraInfo.Id, ownedAura);

                    Logging.LogAura($"Added owned aura {ownedAura.AuraInfo.name} for target: {unit.Name}");

                    RemoveNonStackableAuras(ownedAura);
                }

                if (ownedAura.IsRemoved)
                    return;

                int duration = ownedAura.MaxDuration;
                duration = originalCaster.Spells.ModifyAuraDuration(ownedAura.AuraInfo, unit, duration);

                if (duration != ownedAura.Duration)
                    ownedAura.UpdateDuration(duration, duration);

                ownedAura.UpdateTargets();

                Aura FindOwnedAura()
                {
                    if (ownedAurasById.TryGetValue(auraInfo.Id, out List<Aura> ownedAuraList))
                        foreach (Aura aura in ownedAuraList)
                            if (aura.CasterId == originalCaster.Id)
                                return aura;

                    return null;
                }
            }

            internal void RemoveOwnedAura(Aura aura, AuraRemoveMode removeMode)
            {
                ownedAuras.Remove(aura);
                ownedAurasById.Delete(aura.AuraInfo.Id, aura);

                Logging.LogAura($"Removed owned aura {aura.AuraInfo.name} for target: {unit.Name} with mode: {removeMode}");
            }

            internal void ApplyAuraApplication(AuraApplication auraApplication)
            {
                Logging.LogAura($"Applying application for target: {unit.Name} for aura: {auraApplication.Aura.AuraInfo.name}");

                RemoveNonStackableAuras(auraApplication.Aura);

                auraApplications.Add(auraApplication);
                auraApplicationSet.Add(auraApplication);
                auraApplicationsByAuraId.Insert(auraApplication.Aura.AuraInfo.Id, auraApplication);

                HandleStateContainingAura(auraApplication, true);
                HandleInterruptableAura(auraApplication, true);
                HandleAuraEffects(auraApplication, true);

                auraApplication.Aura.RegisterForTarget(unit, auraApplication);
                unit.VisibleAuras.HandleAuraApplication(auraApplication, true);

                for (int i = 0; i < auraApplication.Aura.AuraInfo.AuraScriptables.Count; i++)
                    auraApplication.Aura.AuraInfo.AuraScriptables[i].AuraApplicationApplied(auraApplication);
            }

            internal void UnapplyAuraApplication(AuraApplication auraApplication, AuraRemoveMode removeMode)
            {
                auraApplicationsByAuraId.Delete(auraApplication.Aura.AuraInfo.Id, auraApplication);
                auraApplications.Remove(auraApplication);
                auraApplicationSet.Remove(auraApplication);

                HandleInterruptableAura(auraApplication, false);
                HandleStateContainingAura(auraApplication, false);
                HandleAuraEffects(auraApplication, false);

                auraApplication.Aura.UnregisterForTarget(unit, auraApplication);
                auraApplication.RemoveMode = removeMode;
                unit.VisibleAuras.HandleAuraApplication(auraApplication, false);

                for (int i = 0; i < auraApplication.Aura.AuraInfo.AuraScriptables.Count; i++)
                    auraApplication.Aura.AuraInfo.AuraScriptables[i].AuraApplicationRemoved(auraApplication);

                Logging.LogAura($"Unapplied application for target: {unit.Name} for aura: {auraApplication.Aura.AuraInfo.name}");
            }

            internal void RemoveNonDeathPersistentAuras()
            {
                List<Aura> aurasToRemove = ownedAuras.FindAll(aura => !aura.AuraInfo.HasAttribute(AuraAttributes.DeathPersistent));
                foreach(Aura aura in aurasToRemove)
                    if (!aura.IsRemoved)
                        aura.Remove(AuraRemoveMode.Death);
            }

            internal void RemoveAurasWithInterrupt(AuraInterruptFlags flags)
            {
                if (!HasAuraAnyInterrupt(flags))
                    return;

                for (int i = 0; i < interruptableAuraApplications.Count; i++)
                    if (interruptableAuraApplications[i].Aura.AuraInfo.InterruptFlags.HasAnyFlag(flags))
                        tempAuraApplications.Add(interruptableAuraApplications[i]);

                foreach (AuraApplication auraApplicationToRemove in tempAuraApplications)
                    RemoveAuraWithApplication(auraApplicationToRemove, AuraRemoveMode.Interrupt);

                tempAuraApplications.Clear();
            }

            internal void RemoveAurasWithCombinedDamageInterrupt(int damageTaken)
            {
                if (!HasAuraAnyInterrupt(AuraInterruptFlags.CombinedDamageTaken))
                    return;

                for (int i = 0; i < interruptableAuraApplications.Count; i++)
                    if (interruptableAuraApplications[i].Aura.AuraInfo.InterruptFlags.HasTargetFlag(AuraInterruptFlags.CombinedDamageTaken))
                    {
                        interruptableAuraApplications[i].HandleDamageInterrupt(damageTaken, out bool removeInstantly);
                        if (removeInstantly)
                            tempAuraApplications.Add(interruptableAuraApplications[i]);
                    }

                foreach (AuraApplication auraApplicationToRemove in tempAuraApplications)
                    RemoveAuraWithApplication(auraApplicationToRemove, AuraRemoveMode.Interrupt);

                tempAuraApplications.Clear();
            }

            private void HandleInterruptableAura(AuraApplication auraApplication, bool added)
            {
                if (!auraApplication.Aura.AuraInfo.HasInterruptFlags)
                    return;

                if (added)
                {
                    interruptableAuraApplications.Add(auraApplication);
                    auraInterruptFlags |= auraApplication.Aura.AuraInfo.InterruptFlags;
                }
                else
                {
                    interruptableAuraApplications.Remove(auraApplication);

                    auraInterruptFlags = 0;
                    foreach (AuraApplication interruptableAura in interruptableAuraApplications)
                        auraInterruptFlags |= interruptableAura.Aura.AuraInfo.InterruptFlags;
                }
            }

            private void HandleStateContainingAura(AuraApplication auraApplication, bool added)
            {
                AuraStateType stateType = auraApplication.Aura.AuraInfo.StateType;
                if (stateType == AuraStateType.None)
                    return;

                if (added)
                {
                    auraApplicationsByAuraState.Insert(stateType, auraApplication);
                    ModifyAuraState(stateType, true);
                }
                else
                {
                    auraApplicationsByAuraState.Delete(stateType, auraApplication);
                    ModifyAuraState(stateType, auraApplicationsByAuraState.ContainsKey(stateType));
                }
            }

            private void HandleAuraEffects(AuraApplication auraApplication, bool added)
            {
                if (added)
                {
                    for (int i = 0; i < auraApplication.Aura.Effects.Count; i++)
                    {
                        if (auraApplication.EffectsToApply.HasBit(i) && !auraApplication.AppliedEffectMask.HasBit(i))
                        {
                            auraEffectsByAuraType.Insert(auraApplication.Aura.Effects[i].EffectInfo.AuraEffectType, auraApplication.Aura.Effects[i]);
                            tempAuraEffectsToHandle.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < auraApplication.Aura.EffectsInfos.Count; i++)
                    {
                        if (auraApplication.AppliedEffectMask.HasBit(i))
                        {
                            auraEffectsByAuraType.Delete(auraApplication.Aura.Effects[i].EffectInfo.AuraEffectType, auraApplication.Aura.Effects[i]);
                            tempAuraEffectsToHandle.Add(i);
                        }
                    }
                }

                for (int i = 0; i < tempAuraEffectsToHandle.Count; i++)
                    auraApplication.HandleEffect(tempAuraEffectsToHandle[i], added, tempAuraHandleGroups);

                tempAuraEffectsToHandle.Clear();
                tempAuraHandleGroups.Clear();
            }

            private void RemoveNonStackableAuras(Aura aura)
            {
                for (int i = 0; i < auraApplications.Count; i++)
                {
                    if (!auraApplications[i].Aura.CanStackWith(aura))
                    {
                        RemoveAuraWithApplication(auraApplications[i], AuraRemoveMode.Default);
                        i = 0;
                    }
                }
            }

            private void RemoveAuraWithApplication(AuraApplication application, AuraRemoveMode mode)
            {
                if (auraApplicationSet.Contains(application))
                {
                    UnapplyAuraApplication(application, mode);

                    if (application.Aura.Owner == unit)
                        application.Aura.Remove(mode);
                }
            }

            private void ModifyAuraState(AuraStateType flag, bool apply)
            {
            }

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                for (int i = 0; i < ownedAuras.Count; i++)
                {
                    Aura auraToUpdate = ownedAuras[i];
                    if (auraToUpdate.Updated)
                        continue;

                    auraToUpdate.DoUpdate(deltaTime, tempApplicationsToRemove);

                    // can be already removed when killing target in periodic tick or scriptable auras
                    if (auraToUpdate.IsExpired && !auraToUpdate.IsRemoved)
                        auraToUpdate.Remove(AuraRemoveMode.Expired);

                    if (i >= ownedAuras.Count || auraToUpdate != ownedAuras[i])
                        i = 0;
                }

                for (int i = 0; i < ownedAuras.Count; i++)
                    ownedAuras[i].LateUpdate();

                foreach(var applicationEntry in tempApplicationsToRemove)
                    if (applicationEntry.Key.RemoveMode == AuraRemoveMode.None)
                        applicationEntry.Key.Target.Auras.RemoveAuraWithApplication(applicationEntry.Key, applicationEntry.Value);

                tempApplicationsToRemove.Clear();
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                while (ownedAuras.Count > 0)
                    ownedAuras[0].Remove(AuraRemoveMode.Detach);

                while (auraApplications.Count > 0)
                    RemoveAuraWithApplication(auraApplications[0], AuraRemoveMode.Detach);

                Assert.IsTrue(auraApplicationsByAuraState.Count == 0);
                Assert.IsTrue(auraEffectsByAuraType.Count == 0);
                Assert.IsTrue(ownedAurasById.Count == 0);
                Assert.IsTrue(auraApplicationsByAuraId.Count == 0);
                Assert.IsTrue(interruptableAuraApplications.Count == 0);
                Assert.IsTrue(auraApplications.Count == 0);
                Assert.IsTrue(auraApplicationSet.Count == 0);
                Assert.IsTrue(ownedAuras.Count == 0);

                unit = null;
            }
        }
    }
}
