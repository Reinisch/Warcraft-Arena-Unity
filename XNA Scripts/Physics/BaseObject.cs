using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public enum ObjectType { Ledge, Impassible, Elevation }
    [Serializable]
    public abstract class BaseObject : ICloneable
    {
        public BoundRectangle BoundRectangle { get; protected set; }
        public ObjectType ObjectType { get; protected set; }

        protected BaseObject(Vector2 position, float w, float h, ObjectType objectType)
        {
            ObjectType = objectType;
            BoundRectangle = new BoundRectangle(position.X, position.Y, w, h);
        }

        public abstract void CheckCollision(IMoveble gameObject, Vector2 playerInput);

        public abstract object Clone();
    }
}