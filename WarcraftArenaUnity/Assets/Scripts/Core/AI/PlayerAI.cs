namespace Core
{
    public sealed class PlayerAI : UnitAI
    {
        private Player Player { get; set; }

        protected override void OnAttach()
        {
            base.OnAttach();

            Player = (Player)Unit;
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            Player = null;
        }
    }
}