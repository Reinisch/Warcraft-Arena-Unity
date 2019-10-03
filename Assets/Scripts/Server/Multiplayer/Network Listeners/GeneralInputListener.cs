using Core;

namespace Server
{
    public partial class PhotonBoltServerListener
    {
        public override void OnEvent(TargetSelectionRequestEvent targetingRequest)
        {
            base.OnEvent(targetingRequest);

            if (targetingRequest.FromSelf)
                return;

            World.FindPlayer(targetingRequest.RaisedBy)?.Attributes.UpdateTarget(targetingRequest.TargetId.PackedValue, updateState: true);
        }

        public override void OnEvent(PlayerEmoteRequestEvent emoteRequest)
        {
            base.OnEvent(emoteRequest);

            var emoteType = (EmoteType)emoteRequest.EmoteType;
            if (!emoteType.IsDefined())
                return;

            World.FindPlayer(emoteRequest.RaisedBy)?.ModifyEmoteState(emoteType);
        }
    }
}
