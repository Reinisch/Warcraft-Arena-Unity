using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class AuraModifier : AuraBase
    {
        public float AddAmount { get; private set; }
        public float MulAmount { get; private set; }

        public AuraModifier(AuraType modifierType , float addAmount, float mulAmount, float seconds, AuraControlEffect controlEffect = AuraControlEffect.None)
            :base(modifierType, controlEffect, seconds)
        {
            AddAmount = addAmount;
            MulAmount = mulAmount;
        }

        public override bool Update(TimeSpan elapsedTime, Entity entity)
        {
            if (TimeLeft == TimeSpan.Zero)
                return true;

            if (Duration == -1)
                return false;

            TimeLeft -= elapsedTime;
            if (TimeLeft.TotalMilliseconds < 0)
            {
                TimeLeft = TimeSpan.Zero;
                return true;
            }
            return false;
        }
        public override int Absorb(DamageType damageType, int damageAmount, Entity entity)
        {
            return 0;
        }
        public override void Apply(Entity entity)
        {
            switch (ModifierType)
            {
                case AuraType.Root:
                    entity.RootStateCount++;
                    break;
                case AuraType.Silence:
                    entity.SilenceStateCount++;
                    break;
                case AuraType.Speed:
                    if(MulAmount > 1f)
                        entity.SpeedBonusMod *= MulAmount;
                    else
                        entity.SpeedPenaltyMod *= MulAmount;
                    break;
                case AuraType.Stun:
                    entity.StunStateCount++;
                    entity.NoControlableStateCount++;
                    break;
                case AuraType.Freeze:
                    entity.FreezeStateCount++;
                    break;
                case AuraType.Control:
                    entity.NoControlableStateCount++;
                    switch (ControlEffect)
                    {
                        case AuraControlEffect.Disorient:
                            entity.DisorientStateCount++;
                            break;
                        case AuraControlEffect.Fear:
                            entity.FearStateCount++;
                            break;
                        case AuraControlEffect.Polymorph:
                            entity.PolymorphStateCount++;
                            break;
                        case AuraControlEffect.Sleep:
                            entity.SleepStateCount++;
                            break;
                    }
                    break;
                case AuraType.HasteRating:
                    entity.HasteRatingAddMod += AddAmount;
                    break;
                case AuraType.DamageReduction:
                    entity.IncomingDamageEfficiencyMulMod *= 1 - AddAmount;
                    break;
                case AuraType.CritChance:
                    entity.CritChanceAddMod += AddAmount;
                    break;
                case AuraType.CritDamageMultiplier:
                    entity.CritDamageMultiplierAddMod += AddAmount;
                    break;
                case AuraType.Invulnerability:
                    entity.InvulnerabilityStateCount++;
                    break;
                case AuraType.Pacify:
                    entity.PacifyStateCount++;
                    break;
                case AuraType.SnareSupression:
                    entity.SnareSupressStateCount++;
                    break;
                case AuraType.NoTargetable:
                    entity.NotTargetableStateCount++;
                    break;
                case AuraType.Invisibility:
                    entity.InvisibleStateCount++;
                    break;
                case AuraType.PhysicalDamageMultiplier:
                    entity.PhysicalDamageMulMod *= MulAmount;
                    break;
                case AuraType.FrostDamageMultiplier:
                    entity.FrostDamageMulMod *= MulAmount;
                    break;
                case AuraType.FireDamageMultiplier:
                    entity.FireDamageMulMod *= MulAmount;
                    break;
                case AuraType.ArcaneDamageMultiplier:
                    entity.ArcaneDamageMulMod *= MulAmount;
                    break;
                case AuraType.NatureDamageMultiplier:
                    entity.NatureDamageMulMod *= MulAmount;
                    break;
                case AuraType.ShadowDamageMultiplier:
                    entity.ShadowDamageMulMod *= MulAmount;
                    break;
                case AuraType.HolyDamageMultiplier:
                    entity.HolyDamageMulMod *= MulAmount;
                    break;
                default:
                    break;
            }
        }
        public override void Reverse(Entity entity)
        {
            switch (ModifierType)
            {
                case AuraType.Root:
                    entity.RootStateCount--;
                    break;
                case AuraType.Silence:
                    entity.SilenceStateCount--;
                    break;
                case AuraType.Speed:
                    if (MulAmount > 1)
                        entity.SpeedBonusMod /= MulAmount;
                    else
                        entity.SpeedPenaltyMod /= MulAmount;
                    break;
                case AuraType.Stun:
                    entity.StunStateCount--;
                    entity.NoControlableStateCount--;
                    break;
                case AuraType.Freeze:
                    entity.FreezeStateCount--;
                    break;
                case AuraType.Control:
                    entity.NoControlableStateCount--;
                    switch (ControlEffect)
                    {
                        case AuraControlEffect.Disorient:
                            entity.DisorientStateCount--;
                            break;
                        case AuraControlEffect.Fear:
                            entity.FearStateCount--;
                            break;
                        case AuraControlEffect.Polymorph:
                            entity.PolymorphStateCount--;
                            break;
                        case AuraControlEffect.Sleep:
                            entity.SleepStateCount--;
                            break;
                    }
                    break;
                case AuraType.HasteRating:
                    entity.HasteRatingAddMod -= AddAmount;
                    break;
                case AuraType.DamageReduction:
                    entity.IncomingDamageEfficiencyMulMod /= 1 - AddAmount;
                    break;
                case AuraType.CritChance:
                    entity.CritChanceAddMod -= AddAmount;
                    break;
                case AuraType.CritDamageMultiplier:
                    entity.CritDamageMultiplierAddMod -= AddAmount;
                    break;
                case AuraType.Invulnerability:
                    entity.InvulnerabilityStateCount--;
                    break;
                case AuraType.Pacify:
                    entity.PacifyStateCount--;
                    break;
                case AuraType.SnareSupression:
                    entity.SnareSupressStateCount--;
                    break;
                case AuraType.NoTargetable:
                    entity.NotTargetableStateCount--;
                    break;
                case AuraType.Invisibility:
                    entity.InvisibleStateCount--;
                    break;
                case AuraType.PhysicalDamageMultiplier:
                    entity.PhysicalDamageMulMod /= MulAmount;
                    break;
                case AuraType.FrostDamageMultiplier:
                    entity.FrostDamageMulMod /= MulAmount;
                    break;
                case AuraType.FireDamageMultiplier:
                    entity.FireDamageMulMod /= MulAmount;
                    break;
                case AuraType.ArcaneDamageMultiplier:
                    entity.ArcaneDamageMulMod /= MulAmount;
                    break;
                case AuraType.NatureDamageMultiplier:
                    entity.NatureDamageMulMod /= MulAmount;
                    break;
                case AuraType.ShadowDamageMultiplier:
                    entity.ShadowDamageMulMod /= MulAmount;
                    break;
                case AuraType.HolyDamageMultiplier:
                    entity.HolyDamageMulMod /= MulAmount;
                    break;
                default:
                    break;
            }
        }
        public override object Clone()
        {
            return new AuraModifier(ModifierType, AddAmount, MulAmount, Duration, ControlEffect);
        }
    }
}