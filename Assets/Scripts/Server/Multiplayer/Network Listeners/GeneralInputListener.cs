using Bolt;
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

        public override void OnEvent(PlayerChatRequestEvent chatRequest)
        {
            base.OnEvent(chatRequest);

            Player player = World.FindPlayer(chatRequest.RaisedBy);
            if (player == null)
                return;

            if (!player.IsAlive)
                return;

            UnitChatMessageEvent unitChatMessageEvent = UnitChatMessageEvent.Create(GlobalTargets.Everyone);
            unitChatMessageEvent.SenderId = player.BoltEntity.NetworkId;
            unitChatMessageEvent.SenderName = player.Name;
            unitChatMessageEvent.Message = chatRequest.Message;
            unitChatMessageEvent.Send();
        }

        public override void OnEvent(PlayerClassChangeRequestEvent classRequest)
        {
            base.OnEvent(classRequest);

            Player player = World.FindPlayer(classRequest.RaisedBy);
            if (player == null)
                return;

            var classType = (ClassType)classRequest.ClassType;
            if (!classType.IsDefined())
                return;

            player.SwitchClass(classType);
        }
    }
}
