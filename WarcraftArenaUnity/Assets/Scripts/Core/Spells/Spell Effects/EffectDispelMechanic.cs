using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Dispel Mechanic", menuName = "Game Data/Spells/Effects/Dispel Mechanic", order = 3)]
    public class EffectDispelMechanic : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, EnumFlag, Header("Dispel Mechanic")]
        private SpellMechanicsFlags mechanicToDispel;

        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.DispelMechanic;
        public SpellMechanicsFlags MechanicToDispel => mechanicToDispel;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectDispelMechanic(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectDispelMechanic(EffectDispelMechanic effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null)
                return;

            var aurasToDispel = new List<Aura>();
            IReadOnlyList<Aura> ownedAuras = target.Auras.OwnedAuras;

            for (int i = 0; i < ownedAuras.Count; i++)
                if (ownedAuras[i].AuraInfo.HasAnyMechanics(effect.MechanicToDispel))
                    aurasToDispel.Add(ownedAuras[i]);

            foreach (Aura aura in aurasToDispel)
                if (!aura.IsRemoved)
                    aura.Remove(AuraRemoveMode.Spell);
        }
    }
}
