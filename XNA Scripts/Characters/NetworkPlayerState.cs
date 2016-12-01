using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Physics;

namespace BasicRpgEngine.Characters
{
    [Serializable]
    public class NetworkPlayerState : IMoveble, ICloneable
    {
        BoundRectangle boundRect;
        Vector2 position;
        Vector2 velocity;
        bool isFlying;
        bool isGrounded;
        bool isKnockedBack;

        public NetworkPlayerState(BoundRectangle boundRect, Vector2 position, bool isFlying, bool isGrounded, bool isKnockedBack, Vector2 velocity)
        {
            this.boundRect = boundRect;
            this.position = position;
            this.isFlying = isFlying;
            this.isGrounded = isGrounded;
            this.isKnockedBack = isKnockedBack;
            this.velocity = velocity;
        }
        public BoundRectangle BoundRect
        {
            get { return boundRect; }
            set { boundRect = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                boundRect.X = position.X + BoundRect.Width;
                boundRect.Y = position.Y;
            }
        }
        public Vector2 Velocity
        { get { return velocity; } set { velocity = value; } }
        public bool IsFlying
        { get { return isFlying; } set { isFlying = value; } }
        public bool IsGrounded
        { get { return isGrounded; } set { isGrounded = value; } }
        public bool IsKnockedBack
        { get { return isKnockedBack; } set { isKnockedBack = value; } }

        public object Clone()
        {
            return new NetworkPlayerState(boundRect, position, isFlying, isGrounded, isKnockedBack, velocity);
        }
    }
}