using System;

namespace Core
{
    public class SpellManager
    {
        public static SpellManager Instance { get; } = new SpellManager();

        public event Action<Unit, SpellInfo> EventSpellCast;
        public event Action<Unit, SpellInfo> EventSpellHit;
        public event Action<Unit, Unit, int, bool> EventSpellDamageDone;

        private SpellManager()
        {
        }

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
        }

        public static void SpellCasted(Unit caster, SpellInfo spellInfo)
        {
            Instance.EventSpellCast?.Invoke(caster, spellInfo);
        }

        public static void SpellHit(Unit unitTarget, SpellInfo spellInfo)
        {
            Instance.EventSpellHit?.Invoke(unitTarget, spellInfo);
        }

        public static void SpellDamageDone(Unit caster, Unit target, int damage, bool isCrit)
        {
            Instance.EventSpellDamageDone?.Invoke(caster, target, damage, isCrit);
        }
    }
}
