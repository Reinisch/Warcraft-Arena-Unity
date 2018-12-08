using System.Collections.Generic;

namespace Core
{
    public class UnitAura : Aura
    {
        public DiminishingGroup AuraDiminishingGroup { get; set; }      // diminishing, allow ApplyAuraHandler to modify and access it

        public UnitAura(SpellInfo spellProto, ulong castId, Unit owner, Unit caster, List<int> baseAmount, ulong casterId)
            : base(spellProto, castId, owner, caster)
        {
            AuraDiminishingGroup = DiminishingGroup.None;
            LoadScripts();
            InitEffects(caster, baseAmount);
            //Owner.AddAura(this, caster);
        }

        public override void ApplyForTarget(Unit target, Unit caster, AuraApplication aurApp) { }
        public override void UnapplyForTarget(Unit target, Unit caster, AuraApplication aurApp) { }
        public override void Remove(AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public override void FillTargetMap(Dictionary<Unit, uint> targets, Unit caster) { }
    }
}