namespace Core
{
    public class EffectKill : SpellEffectInfo
    {
        public override float Value => 1;
        public override SpellEffectType EffectType => SpellEffectType.Kill;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
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
