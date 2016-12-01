using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public class StaticImpassableObject : BaseObject
    {
        public StaticImpassableObject(Vector2 position, float w, float h)
            : base(position,w,h,ObjectType.Impassible){}

        public override void CheckCollision(IMoveble gameObject, Vector2 playerInput)
        {
            if (BoundRectangle.CollideCheck(gameObject))
            {
                if (gameObject.BoundRect.Bottom <= BoundRectangle.Top)
                {
                    gameObject.Velocity = new Vector2(gameObject.Velocity.X, BoundRectangle.Top - gameObject.BoundRect.Bottom);
                    gameObject.IsGrounded = true;
                }
                else if (gameObject.BoundRect.Top >= BoundRectangle.Bottom)
                {
                    gameObject.Velocity = new Vector2(gameObject.Velocity.X, BoundRectangle.Bottom - gameObject.BoundRect.Top);
                }

                if (gameObject.BoundRect.Right <= BoundRectangle.Left)
                {
                    gameObject.Velocity = new Vector2(BoundRectangle.Left - gameObject.BoundRect.Right, gameObject.Velocity.Y);
                }
                else if (gameObject.BoundRect.Left >= BoundRectangle.Right)
                {
                    gameObject.Velocity = new Vector2(BoundRectangle.Right - gameObject.BoundRect.Left, gameObject.Velocity.Y);
                }
            }
        }

        public override object Clone()
        {
            return new StaticImpassableObject(new Vector2(BoundRectangle.X, BoundRectangle.Y), BoundRectangle.Width, BoundRectangle.Height);
        }
    }
}