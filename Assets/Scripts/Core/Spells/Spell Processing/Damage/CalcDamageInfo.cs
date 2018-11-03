using System;

namespace Core
{
    public class CalcDamageInfo
    {
        public Unit Attacker { get; set; }
        public Unit Target { get; set; }
        public uint DamageSchoolMask { get; set; }
        public uint Damage { get; set; }
        public uint Absorb { get; set; }
        public uint Resist { get; set; }
        public uint BlockedAmount { get; set; }
        public uint HitInfo { get; set; }
        public uint TargetState { get; set; }

        public WeaponAttackType AttackType { get; set; }
        public uint ProcAttacker { get; set; }
        public uint ProcVictim { get; set; }
        public uint ProcEx { get; set; }
        public uint CleanDamage { get; set; }
        public MeleeHitOutcome MeleeHitOutcome { get; set; }

        public CalcDamageInfo(Unit attacker, Unit target, uint spellId, uint schoolMask, Guid castId = default(Guid))
        {

        }
    }
}