namespace Core
{
    public class EffectKill : SpellEffectInfo
    {
        public override SpellEffectType EffectType => SpellEffectType.Kill;
        public override SpellExplicitTargetType ExplicitTargetType => SpellExplicitTargetType.Target;
        public override SpellTargetEntities TargetEntityType => SpellTargetEntities.Unit;

        internal override void Handle(Spell spell, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectKill(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectKill(EffectKill effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget || target == null || !target.IsAlive)
                return;

            Caster.Kill(target);
        }
    }
}
