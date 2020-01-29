using Common;
using Core;

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

        public override void OnEvent(PlayerMovementControlChanged movementControlChangeEvent)
        {
            base.OnEvent(movementControlChangeEvent);

            LocalPlayer?.Handle(movementControlChangeEvent);
        }

        public override void OnEvent(UnitChatMessageEvent unitChatMessageEvent)
        {
            base.OnEvent(unitChatMessageEvent);

            Unit who = World.UnitManager.Find(unitChatMessageEvent.SenderId.PackedValue);
            if (who != null)
                EventHandler.ExecuteEvent(GameEvents.UnitChat, who, unitChatMessageEvent.Message);
        }
    }
}
