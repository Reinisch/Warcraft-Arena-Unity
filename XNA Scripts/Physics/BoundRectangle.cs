using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public struct BoundRectangle
    {
        public float X
        { get; set; }
        public float Y
        { get; set; }
        public float Width
        { get; set; }
        public float Height
        { get; set; }

        public float Right
        { get { return X + Width; } }
        public float Left
        { get { return X; } }
        public float Top
        { get { return Y; } }
        public float Bottom
        { get { return Y + Height; } }

        public Vector2 BottomCenter
        { get { return new Vector2(X + Width / 2, Y + Height); } }
        public Vector2 TopCenter
        { get { return new Vector2(X + Width / 2, Y); } }
        public Vector2 Center
        { get { return new Vector2(X + Width / 2, Y + Height / 2); } }

        public BoundRectangle(float x, float y, float width, float height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool CollideCheck(IMoveble gameObject)
        {
            BoundRectangle box = gameObject.BoundRect + gameObject.Velocity;

            if ((Right >= box.Left) && (Left <= box.Right))
            {
                if ((Bottom >= box.Top) && (Top <= box.Bottom))
                {
                    return true;
                }
            }

            return false;
        }

        public static BoundRectangle operator +(BoundRectangle box, Vector2 Addition)
        {
            BoundRectangle newRect = box;
            newRect.X += Addition.X;
            newRect.Y += Addition.Y;
            return newRect;
        }
    }
}