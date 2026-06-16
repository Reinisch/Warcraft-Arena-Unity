using Core;

namespace Net
{
    /// <summary>Server → controlling client: a movement speed rate changed. (Bolt: PlayerSpeedRateChangedEvent)</summary>
    public readonly struct PlayerSpeedRateChanged : INetMessage
    {
        public UnitMoveType MoveType { get; }
        public float SpeedRate { get; }

        public PlayerSpeedRateChanged(UnitMoveType moveType, float speedRate)
        {
            MoveType = moveType;
            SpeedRate = speedRate;
        }
    }
}
