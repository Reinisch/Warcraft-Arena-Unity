using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public class StaticElevationObject : BaseObject
    {
        public StaticElevationObject(Vector2 position, float w, float h)
            : base(position, w, h, ObjectType.Elevation){}

        public override void CheckCollision(IMoveble gameObject, Vector2 playerInput)
        {
            if (gameObject.IsKnockedBack)
                return;
            if (BoundRectangle.CollideCheck(gameObject))
            {
                if (gameObject.BoundRect.Bottom > BoundRectangle.Bottom)
                    return;
                gameObject.Velocity = new Vector2(gameObject.Velocity.X, gameObject.Velocity.Y);
                gameObject.IsGrounded = true;
            }

        }

        public override object Clone()
        {
            return new StaticElevationObject(new Vector2(BoundRectangle.X, BoundRectangle.Y), BoundRectangle.Width, BoundRectangle.Height);
        }
    }
}