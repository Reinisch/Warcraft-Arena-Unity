using Core;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Server → controlling client: hand movement authority to/from the client. When granted, carries the
    /// authoritative position/flags to snap to first. (Bolt: PlayerMovementControlChanged)
    /// </summary>
    public readonly struct PlayerMovementControlChanged : INetMessage
    {
        public bool PlayerHasControl { get; }
        public Vector3 LastServerPosition { get; }
        public MovementFlags LastServerMovementFlags { get; }

        public PlayerMovementControlChanged(bool playerHasControl, Vector3 lastServerPosition, MovementFlags lastServerMovementFlags)
        {
            PlayerHasControl = playerHasControl;
            LastServerPosition = lastServerPosition;
            LastServerMovementFlags = lastServerMovementFlags;
        }
    }
}
