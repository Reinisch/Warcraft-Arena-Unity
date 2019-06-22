using System.Collections.Generic;
using Common;

namespace Core
{
    public class Aura
    {
        private int maxDuration;
        private bool isSingleTarget;

        private Dictionary<ulong, AuraApplication> applications = new Dictionary<ulong, AuraApplication>();
        public List<AuraEffect> AuraEffects { get; } = new List<AuraEffect>();
        public List<SpellEffectInfo> SpellEffects { get; } = new List<SpellEffectInfo>();
        public SpellInfo SpellInfo { get; private set; }
        public ulong CastId { get; private set; }
        public ulong CasterId { get; private set; }
        public int SpellVisualId { get; private set; }

        public Unit Caster => null;
        public Unit Owner { get; private set; }

        public Aura(SpellInfo spellproto, ulong castId, Unit owner, Unit caster)
        {
        }

        #region Aura types

        public bool HasMoreThanOneEffectForType(AuraType auraType)
        {
            return false;
        }

        public bool IsArea()
        {
            return false;
        }

        public bool IsPassive()
        {
            return false;
        }

        public bool IsRemoved()
        {
            return false;
        }

        public bool IsSingleTarget()
        {
            return isSingleTarget;
        }

        public bool IsSingleTargetWith(Aura aura)
        {
            return false;
        }

        public void SetIsSingleTarget(bool val)
        {
            isSingleTarget = val;
        }

        #endregion

        #region Application, updates and removal

        public static Aura TryRefreshStackOrCreate(SpellInfo spellProto, Unit owner, ref bool refresh, List<int> baseAmount, ulong castId, ulong targetCasterId, ulong originalCasterId)
        {
            Assert.IsNotNull(spellProto);
            Assert.IsNotNull(owner);
            Assert.IsTrue(originalCasterId != 0 || targetCasterId != 0);

            if (refresh)
                refresh = false;

            var foundAura = owner.TryStackingOrRefreshingExistingAura(spellProto, originalCasterId, targetCasterId, baseAmount);
            if (foundAura == null)
                return Create(spellProto, owner, baseAmount, castId, originalCasterId, targetCasterId);

            if (foundAura.IsRemoved())
                return null;

            refresh = true;
            return foundAura;
        }

        public static Aura Create(SpellInfo spellProto, Unit owner, List<int> baseAmount, ulong castId, ulong originalCasterId, ulong targetCasterId)
        {
            Assert.IsNotNull(spellProto);
            Assert.IsNotNull(owner);
            Assert.IsTrue(originalCasterId != 0 || targetCasterId != 0);

            if (originalCasterId == 0)
                originalCasterId = targetCasterId;

            Unit caster = owner.Id == originalCasterId ? owner : owner.Map.FindMapEntity<Unit>(originalCasterId);
            Aura aura = new UnitAura(spellProto, owner, caster, baseAmount, castId);
            return aura.IsRemoved() ? null : aura;
        }

        public virtual void ApplyForTarget(Unit target, Unit caster, AuraApplication auraApp)
        {
        }

        public virtual void UnapplyForTarget(Unit target, Unit caster, AuraApplication auraApp)
        {
        }

        public virtual void Remove(AuraRemoveMode removeMode = AuraRemoveMode.Default)
        {
        }

        #endregion

        #region Charges and stacks

        public void SetCharges(ushort charges)
        {
        }

        public ushort CalcMaxCharges(Unit caster)
        {
            return 0;
        }

        public ushort CalcMaxCharges()
        {
            return CalcMaxCharges(Caster);
        }

        public bool ModCharges(int num, AuraRemoveMode removeMode = AuraRemoveMode.Default)
        {
            return true;
        }

        public bool DropCharge(AuraRemoveMode removeMode = AuraRemoveMode.Default)
        {
            return ModCharges(-1, removeMode);
        }

        public void SetStackAmount(uint num)
        {
        }

        public bool ModStackAmount(int num, AuraRemoveMode removeMode = AuraRemoveMode.Default)
        {
            return false;
        }

        #endregion
    }
}