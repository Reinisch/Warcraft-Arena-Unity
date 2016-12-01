using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Graphics;
using BasicRpgEngine;

namespace BasicRpgEngine.Physics
{
    public abstract class Projectile : IDisposable
    {
        protected BoundRectangle boundRect;
        protected Vector2 position;
        protected AnimatedSprite projectileSprite;

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
                boundRect.X = position.X;
                boundRect.Y = position.Y;
            }
        }
        public bool NeedsDispose
        { get; set; }

        public abstract bool Update(TimeSpan elapsedGameTime, World world);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Dispose();
    }
}