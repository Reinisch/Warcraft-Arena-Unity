using System;
using System.Collections.Generic;
using Common;
using JetBrains.Annotations;

namespace Core
{
    public class Aura
    {
        private const int UpdateTargetInterval = 500;
        private static int AuraAliveCount;

        private readonly List<Unit> tempRemovableTargets = new List<Unit>();
        private readonly HashSet<Unit> tempUpdatedTargets = new HashSet<Unit>();
        private readonly List<AuraEffect> effects = new List<AuraEffect>();
        private readonly List<AuraEffectInfo> effectInfos = new List<AuraEffectInfo>();
        private readonly List<AuraApplication> applications = new List<AuraApplication>();
        private readonly Dictionary<ulong, AuraApplication> applicationsByTargetId = new Dictionary<ulong, AuraApplication>();

        private int updateInvervalLeft;
        private readonly IReadOnlyReference<Unit> casterReference;

        internal bool Updated { get; private set; }

        public IReadOnlyDictionary<ulong, AuraApplication> ApplicationsByTargetId => applicationsByTargetId;
        public IReadOnlyList<AuraApplication> Applications => applications;
        public IReadOnlyList<AuraEffectInfo> EffectsInfos => effectInfos;
        public IReadOnlyList<AuraEffect> Effects => effects;

        [CanBeNull]
        public Unit Caster => casterReference.Value;
        [NotNull]
        public Unit Owner { get; }
        [NotNull]
        public AuraInfo AuraInfo { get; }
        [NotNull]
        public SpellInfo SpellInfo { get; }

        public ulong CasterId { get; }
        public int Charges { get; private set; }
        public int Duration { get; private set; }
        public int MaxDuration { get; private set; }

        public int RefreshDuration { get; private set; }
        public int RefreshServerFrame { get; private set; }
        public bool IsRemoved { get; private set; }
        public bool IsExpired => Duration == 0;

        internal Aura(AuraInfo auraInfo, SpellInfo spellInfo, [NotNull] Unit owner, [NotNull] Unit caster)
        {
            AuraInfo = auraInfo;
            SpellInfo = spellInfo;
            casterReference = caster.SelfReference;
            Owner = owner;
            CasterId = caster.Id;

            UpdateDuration(auraInfo.Duration, auraInfo.MaxDuration);
            UpdateCharges(AuraInfo.Charges);

            effectInfos.AddRange(auraInfo.AuraEffects);

            for (int index = 0; index < effectInfos.Count; index++)
            {
                AuraEffect newEffect = effectInfos[index].CreateEffect(this, Caster, index);
                newEffect.CalculateValue();
                effects.Add(newEffect);
            }

            Logging.LogAura($"Created aura {AuraInfo.name} for target: {Owner.Name}, current count: {++AuraAliveCount}");
        }

        ~Aura()
        {
            Logging.LogAura($"Finalized aura, current count: {--AuraAliveCount}");
        }

        internal void DoUpdate(int deltaTime, Dictionary<AuraApplication, AuraRemoveMode> applicationsToRemove)
        {
            Updated = true;

            if (Duration > 0 && (Duration -= deltaTime) < 0)
                Duration = 0;

            if (updateInvervalLeft <= deltaTime)
                UpdateTargets();
            else
                updateInvervalLeft -= deltaTime;

            foreach (AuraEffect effect in effects)
                effect.DoUpdate(deltaTime);

            // can be already removed when killing target in periodic tick or scriptable auras
            if(!IsRemoved)
                foreach (AuraApplication application in applications)
                    application.DoUpdate(deltaTime, applicationsToRemove);
        }

        internal void LateUpdate()
        {
            Updated = false;
        }

        internal void RegisterForTarget(Unit target, AuraApplication auraApplication)
        {
            applications.Add(auraApplication);
            applicationsByTargetId.Add(target.Id, auraApplication);
        }

        internal void UnregisterForTarget(Unit target, AuraApplication auraApplication)
        {
            applications.Remove(auraApplication);
            applicationsByTargetId.Remove(target.Id);
        }

        internal void Remove(AuraRemoveMode removeMode = AuraRemoveMode.Default)
        {
            Assert.IsFalse(IsRemoved, $"Aura {AuraInfo.Id} is removed twice!");

            IsRemoved = true;
            Charges = 0;

            while (applications.Count > 0)
            {
                AuraApplication applicationToRemove = applications[0];
                applicationToRemove.Target.Auras.UnapplyAuraApplication(applicationToRemove, removeMode);
            }

            Owner.Auras.RemoveOwnedAura(this, removeMode);
        }

        internal void UpdateTargets()
        {
            updateInvervalLeft = UpdateTargetInterval;

            switch (AuraInfo.TargetingMode)
            {
                case AuraTargetingMode.Single:
                    AddUnitToAura(Owner);
                    break;
                case AuraTargetingMode.AreaFriend:
                    throw new NotImplementedException();
                case AuraTargetingMode.AreaEnemy:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // remove targets not present in new update
            for (int i = applications.Count - 1; i >= 0; i--)
                if (!tempUpdatedTargets.Contains(applications[i].Target))
                    tempRemovableTargets.Add(applications[i].Target);

            // unapply aura for removed targets
            foreach (Unit removableUnit in tempRemovableTargets)
                if (applicationsByTargetId.TryGetValue(removableUnit.Id, out AuraApplication removableApplication))
                    removableUnit.Auras.UnapplyAuraApplication(removableApplication, AuraRemoveMode.Default);

            tempUpdatedTargets.Clear();
            tempRemovableTargets.Clear();
        }

        internal void UpdateDuration(int duration, int maxDuration)
        {
            Duration = duration;
            MaxDuration = maxDuration;

            RefreshServerFrame = BoltNetwork.ServerFrame;
            RefreshDuration = Duration;

            Owner.VisibleAuras.NeedUpdate = true;
        }

        internal void UpdateCharges(int charges)
        {
            int oldCharges = Charges;
            Charges = Math.Min(AuraInfo.MaxCharges, charges);

            if (oldCharges != Charges)
                Owner.VisibleAuras.NeedUpdate = true;
        }

        internal void DropCharge()
        {
            if (AuraInfo.UsesCharges && Charges > 0)
            {
                if (--Charges == 0)
                    Remove(AuraRemoveMode.Expired);

                Owner.VisibleAuras.NeedUpdate = true;
            }
        }

        internal void Refresh()
        {
            UpdateCharges(Charges + AuraInfo.Charges);

            for (int i = 0; i < Effects.Count; i++)
                for (int j = 0; j < Applications.Count; j++)
                    Effects[i].HandleEffect(Applications[j], AuraEffectHandleMode.Refresh, true);
        }

        internal bool CanStackWith(Aura existingAura)
        {
            if (this == existingAura)
                return true;

            bool sameCaster = CasterId == existingAura.CasterId;
            if (!sameCaster && AuraInfo == existingAura.AuraInfo && !AuraInfo.HasAttribute(AuraAttributes.StackForAnyCasters))
                return false;

            return true;
        }

        internal bool CanDispel(Aura otherAura)
        {
            // check for auras that ignore immunities and can't be dispelled
            if (!SpellInfo.CanDispelAura(otherAura.SpellInfo))
                return false;

            // don't dispel self
            if (this == otherAura)
                return false;

            // don't dispel passives
            if (otherAura.SpellInfo.IsPassive)
                return false;

            // don't dispel same positivity effects
            if (otherAura.AuraInfo.IsPositive == AuraInfo.IsPositive)
                return false;

            return true;
        }

        private void AddUnitToAura(Unit unit)
        {
            tempUpdatedTargets.Add(unit);

            if (applicationsByTargetId.ContainsKey(unit.Id) || unit.IsImmuneToAura(AuraInfo, Caster))
                return;

            // check effect for immunity
            int auraEffectMask = 0;
            for (int i = 0; i < effectInfos.Count; i++)
                if (!unit.IsImmuneToAuraEffect(effectInfos[i], Caster))
                    auraEffectMask = auraEffectMask.SetBit(i);

            if (auraEffectMask == 0)
                return;

            // check for non stackable auras
            if (unit != Owner)
                for (int i = 0; i < unit.AuraApplications.Count; i++)
                    if (!CanStackWith(unit.AuraApplications[i].Aura))
                        return;

            unit.Auras.ApplyAuraApplication(new AuraApplication(unit, this, auraEffectMask));
        }
    }
}