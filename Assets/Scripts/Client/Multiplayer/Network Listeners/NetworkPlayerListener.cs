namespace Client
{
    public partial class PhotonBoltClientListener
    {
        public override void OnEvent(PlayerSpeedRateChangedEvent speedChangeEvent)
        {
            base.OnEvent(speedChangeEvent);

            LocalPlayer?.Handle(speedChangeEvent);
        }
    }
}
