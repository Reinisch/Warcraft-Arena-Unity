using UnityEngine;

namespace Net
{
    /// <summary>Server → controlling client: teleport the player to a position. (Bolt: SpellPlayerTeleportEvent)</summary>
    public readonly struct SpellPlayerTeleport : INetMessage
    {
        public Vector3 TargetPosition { get; }

        public SpellPlayerTeleport(Vector3 targetPosition)
        {
            TargetPosition = targetPosition;
        }
    }
}
