using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public class StaticLedgeObject : BaseObject
    {
        public StaticLedgeObject(Vector2 position, float w, float h)
            : base(position, w, h, ObjectType.Ledge){}

        public override void CheckCollision(IMoveble gameObject, Vector2 playerInput)
        {
            if (BoundRectangle.CollideCheck(gameObject))
            {
                if ((gameObject.BoundRect.Bottom == BoundRectangle.Top) && (playerInput.Y > 0))
                {
                    gameObject.Velocity = new Vector2(gameObject.Velocity.X, gameObject.Velocity.Y);
                    return;
                }

                if (gameObject.BoundRect.Bottom <= BoundRectangle.Top)
                {
                    if (playerInput.Y <= 0)
                    {
                        gameObject.Velocity = new Vector2(gameObject.Velocity.X, BoundRectangle.Top - gameObject.BoundRect.Bottom);
                        gameObject.IsGrounded = true;
                    }
                }
            }

        }

        public override object Clone()
        {
            return new StaticLedgeObject(new Vector2(BoundRectangle.X, BoundRectangle.Y), BoundRectangle.Width, BoundRectangle.Height);
        }
    }
}