using System;
using Client;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Player : Unit
    {
        [SerializeField, UsedImplicitly, Header("Player"), Space(10)] private WarcraftController controller;

        public GridReference<Player> GridReference { get; } = new GridReference<Player>();
        public WarcraftController Controller => controller;
        
        public override bool AutoScoped => true;
        public override EntityType EntityType => EntityType.Player;
        public override DeathState DeathState { get => base.DeathState; set => throw new NotImplementedException(); }

        public override bool IsImmunedToSpellEffect(SpellInfo spellInfo, int index)
        {
            return false;
        }

        public override void Accept(IUnitVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Target methods

        public Unit GetSelectedUnit() { throw new NotImplementedException(); }
        public Player GetSelectedPlayer() { throw new NotImplementedException(); }

        public override void SetTarget(ulong targetId) { } // Used for serverside target changes, does not apply to players
        public void SetSelection(ulong targetId) { SetULongValue(EntityFields.Target, targetId); }

        #endregion

        #region Spell modifiers and potions

        public void AddSpellMod(SpellModifier mod, bool apply) { throw new NotImplementedException(); }
        public bool IsAffectedBySpellmod(SpellInfo  spellInfo, SpellModifier mod, Spell spell = null) { throw new NotImplementedException(); }
        public void ApplySpellMod(int spellId, SpellModifierType op, ref float basevalue, Spell spell = null) { throw new NotImplementedException(); }
        public void ApplySpellMod(int spellId, SpellModifierType op, ref int basevalue, Spell spell = null) { throw new NotImplementedException(); }
        public void RemoveSpellMods(Spell spell) { throw new NotImplementedException(); }
        public void RestoreSpellMods(Spell spell, uint ownerAuraId = 0, Aura aura = null) { throw new NotImplementedException(); }
        public void RestoreAllSpellMods(uint ownerAuraId = 0, Aura aura = null) { throw new NotImplementedException(); }
        public void DropModCharge(SpellModifier mod, Spell spell) { throw new NotImplementedException(); }
        public void SetSpellModTakingSpell(Spell spell, bool apply) { throw new NotImplementedException(); }

        #endregion
    }
}