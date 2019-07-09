namespace Server
{
    public partial class PhotonBoltServerListener
    {
        public override void OnEvent(TargetSelectionRequestEvent targetingRequest)
        {
            base.OnEvent(targetingRequest);

            World.FindPlayer(targetingRequest.RaisedBy)?.UpdateTarget(targetingRequest.TargetId.PackedValue, updateState: true);
        }
    }
}
