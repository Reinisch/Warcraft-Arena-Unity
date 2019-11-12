namespace Core
{
    internal struct SpellCastingOptions
    {
        public SpellExplicitTargets Targets { get; private set; }
        public SpellCastFlags SpellFlags  { get; private set; }
        public MovementFlags? MovementFlags  { get; private set; }

        public SpellCastingOptions(SpellExplicitTargets targets = null, SpellCastFlags castFlags = 0, MovementFlags? movementFlags = null)
        {
            Targets = targets;
            SpellFlags = castFlags;
            MovementFlags = movementFlags;
        }
    }
}
