namespace Client
{
    public partial class PhotonBoltClientListener
    {
        public override void OnEvent(PlayerSpeedRateChangedEvent speedChangeEvent)
        {
            base.OnEvent(speedChangeEvent);

            LocalPlayer?.Handle(speedChangeEvent);
        }

        public override void OnEvent(PlayerRootChangedEvent rootChangeEvent)
        {
            base.OnEvent(rootChangeEvent);

            LocalPlayer?.Handle(rootChangeEvent);
        }
    }
}
