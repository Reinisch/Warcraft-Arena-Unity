using Core;

namespace Net
{
    /// <summary>
    /// Entity-scoped: a unit took spell damage, for observers to play wound/hit reactions.
    /// Routed via <see cref="NetTarget.Observers"/>. (Bolt: UnitSpellDamageEvent)
    /// </summary>
    public readonly struct UnitSpellDamage : INetMessage
    {
        public NetId CasterId { get; }
        public int Damage { get; }
        public HitType HitType { get; }

        public UnitSpellDamage(NetId casterId, int damage, HitType hitType)
        {
            CasterId = casterId;
            Damage = damage;
            HitType = hitType;
        }
    }
}
