using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Physics
{
    public enum CameraMode { Free, Follow }

    [Serializable]
    public class Camera
    {
        Vector2 position;
        Point mapSize;
        float speed;
        float zoom;
        Rectangle viewportRectangle;
        CameraMode mode;

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateScale(zoom) *
                    Matrix.CreateTranslation(new Vector3(-Position, 0f));
            }
        }
        public Rectangle ViewportRectangle
        {
            get
            {
                return new Rectangle
                    (viewportRectangle.X,
                    viewportRectangle.Y,
                    viewportRectangle.Width,
                    viewportRectangle.Height);
            }
        }
        public Point MapSize
        {
            get { return mapSize; }
            set { mapSize = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            private set { position = value; }
        }
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = (float)MathHelper.Clamp(speed, 1.0f, 16.0f);
            }
        }
        public float Zoom
        {
            get { return zoom; }
        }
        public CameraMode CameraMode
        {
            get { return mode; }
        }

        public Camera(Rectangle viewportRect, Point mapsize)
        {
            speed = 8.0f;
            zoom = 1.0f;
            viewportRectangle = viewportRect;
            Position = Vector2.Zero;
            this.mapSize = mapsize;
            mode = CameraMode.Follow;
        }

        public void Update(GameTime gameTime)
        {
            if (mode == CameraMode.Follow)
                return;

            Vector2 motion = Vector2.Zero;

            if (InputHandler.KeyDown(Keys.Left))
                motion.X = -speed;
            else if (InputHandler.KeyDown(Keys.Right))
                motion.X = speed;

            if (InputHandler.KeyDown(Keys.Up))
                motion.Y = -speed;
            else if (InputHandler.KeyDown(Keys.Down))
                motion.Y = speed;

            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                position += motion * speed;
                LockCamera();
            }
        }
        private void LockCamera()
        {
            position.X = MathHelper.Clamp(position.X, 0, mapSize.X * zoom - viewportRectangle.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, mapSize.Y * zoom - viewportRectangle.Height);
        }
        public void ZoomIn()
        {
            zoom += .025f;
            if (zoom > 3f)
                zoom = 3f;

            Vector2 newPosition = Position * zoom;
            SnapToPosition(newPosition);
        }
        public void ZoomOut()
        {
            zoom -= .025f;
            if (zoom < .9f)
                zoom = .9f;

            Vector2 newPosition = Position * zoom;
            SnapToPosition(newPosition);
        }
        private void SnapToPosition(Vector2 newPosition)
        {
            position.X = newPosition.X - viewportRectangle.Width / 2;
            position.Y = newPosition.Y - viewportRectangle.Height / 2;
            LockCamera();
        }
        public void LockSprite(NetworkPlayer localPlayer)
        {
            position.X = (localPlayer.BoundRect.BottomCenter.X) * zoom - (viewportRectangle.Width / 2);
            position.Y = localPlayer.BoundRect.Top * zoom - viewportRectangle.Height / 2;
            LockCamera();
        }
        public CameraMode SwitchCameraMode()
        {
            if (mode == CameraMode.Follow)
            {
                mode = CameraMode.Free;
                return CameraMode.Free;
            }
            else
            {
                mode = CameraMode.Follow;
                return CameraMode.Follow;
            }
        }
    }
}