namespace Core
{
    public abstract class UnitAI
    {
        protected UnitAI(Unit unit)
        {
        }
        
        public abstract void DoUpdate(int deltaTime);

        public virtual void InitializeAI()
        {
        }

        public virtual void DeinitializeAI()
        {
        }

        public virtual void DoAction(int actionId)
        {
        }

        public virtual void DamageDealt(Unit victim, int damage, SpellDamageType spellDamageType)
        {
        }

        public virtual void DamageTaken(Unit attacker, int damage)
        {
        }

        public void DoCast(int spellId)
        {
        }

        public void DoCast(Unit target, int spellId, bool triggered = false)
        {
        }
    }
}
