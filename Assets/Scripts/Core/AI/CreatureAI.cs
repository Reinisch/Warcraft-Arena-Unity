namespace Core
{
    public sealed class CreatureAI : UnitAI
    {
        private Creature Creature { get; set; }

        protected override void OnAttach()
        {
            base.OnAttach();

            Creature = (Creature)Unit;
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            Creature = null;
        }
    }
}