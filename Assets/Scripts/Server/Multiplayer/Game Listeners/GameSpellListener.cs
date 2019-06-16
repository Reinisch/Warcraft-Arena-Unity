using Common;
using Core;

namespace Server
{
    internal class GameSpellListener : BaseGameListener
    {
        internal GameSpellListener(WorldServerManager worldServerManager) : base(worldServerManager)
        {
            EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
        }

        internal override void Dispose()
        {
            EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, bool isCrit)
        {
            if (caster is Player player && player.BoltEntity.Controller != null)
            {
                SpellDamageDoneEvent spellDamageEvent = SpellDamageDoneEvent.Create(player.BoltEntity.Controller, Bolt.ReliabilityModes.ReliableOrdered);
                spellDamageEvent.Target = target.BoltEntity.NetworkId;
                spellDamageEvent.DamageAmount = damageAmount;
                spellDamageEvent.IsCrit = isCrit;
                spellDamageEvent.Send();
            }
        }
    }
}
