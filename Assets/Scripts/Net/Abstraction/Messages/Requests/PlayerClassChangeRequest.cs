using Core;

namespace Net
{
    /// <summary>Client → server: switch the player's class. (Bolt: PlayerClassChangeRequestEvent)</summary>
    public readonly struct PlayerClassChangeRequest : INetMessage
    {
        public ClassType ClassType { get; }

        public PlayerClassChangeRequest(ClassType classType)
        {
            ClassType = classType;
        }
    }
}
