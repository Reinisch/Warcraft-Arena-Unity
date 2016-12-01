using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    public interface IMoveble
    {
        BoundRectangle BoundRect { get; set; }

        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }

        bool IsFlying { get; set; }
        bool IsGrounded { get; set; }
        bool IsKnockedBack { get; set; }
    }
}