using System.Collections.Generic;

namespace Core
{
    public class UnitAura : Aura
    {
        public UnitAura(SpellInfo spellProto, Unit owner, Unit caster, List<int> baseAmount, ulong castId) : base(spellProto, castId, owner, caster)
        {
            LoadScripts();

            InitEffects(caster, baseAmount);
            Owner.AddAura(this, caster);
        }

        public override void ApplyForTarget(Unit target, Unit caster, AuraApplication aurApp) { }
        public override void UnapplyForTarget(Unit target, Unit caster, AuraApplication aurApp) { }
        public override void Remove(AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public override void FillTargetMap(Dictionary<Unit, uint> targets, Unit caster) { }
    }
}