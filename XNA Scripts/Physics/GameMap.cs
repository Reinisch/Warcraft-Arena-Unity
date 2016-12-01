using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Physics
{
    public class GameMap
    {
        private Rectangle ScreenRectangle
        { get; set; }

        public string Name
        { get; private set; }
        public int MapWidth
        { get; private set; }
        public int MapHeight
        { get; private set; }
        public BaseObject[] MapObjects
        { get; private set; }
        public MapPart[,] MapParts
        { get; private set; }

        public GameMap(MapPart[,] mapLayers, BaseObject[] mapObjects, Rectangle screenRectangle, string name)
        {
            Name = name;
            MapWidth = mapLayers[0,0].Width * mapLayers.GetLength(1);
            MapHeight = mapLayers[0,0].Height * mapLayers.GetLength(0);
            MapObjects = mapObjects;
            ScreenRectangle = screenRectangle;
            MapParts = mapLayers;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            Point min; Point max;
            min.X = Math.Max(0, (int)(camera.Position.X / camera.Zoom) / MapParts[0,0].Width);
            min.Y = Math.Max(0, (int)(camera.Position.Y / camera.Zoom) / MapParts[0, 0].Height);
            max.X = Math.Min( (int)((camera.Position.X + camera.ViewportRectangle.Width) / camera.Zoom + 1)/ MapParts[0,0].Width,
                MapWidth / MapParts[0,0].Width - 1);
            max.Y = Math.Min((int)((camera.Position.Y + camera.ViewportRectangle.Height) / camera.Zoom + 1) / MapParts[0, 0].Height,
                MapHeight / MapParts[0, 0].Height - 1);

            Rectangle dest = new Rectangle(min.X*MapParts[0,0].Width, min.Y*MapParts[0,0].Height,
                 MapParts[0, 0].Width, MapParts[0, 0].Height);

            for (int i = min.Y; i <= max.Y; i++)
            {
                for(int j = min.X; j <= max.X; j++)
                {
                    spriteBatch.Draw(MapParts[i,j].BackgroundImage, dest, MapParts[i,j].Color);
                    dest.X += MapParts[0, 0].Width;
                }
                dest.X = min.X * MapParts[0, 0].Width;
                dest.Y += MapParts[0, 0].Height;
            }
        }

        public bool GetNearestElevetion(Vector2 position, out Vector2 nearestElevation)
        {
            bool success = false;
            float minDistanse = float.MaxValue;
            float temp = 0;
            nearestElevation = Vector2.Zero;

            foreach (BaseObject baseObject in MapObjects)
            {
                if (baseObject.ObjectType == ObjectType.Elevation)
                {
                    temp = Vector2.Distance(baseObject.BoundRectangle.TopCenter, position);
                    if (temp < minDistanse)
                    {
                        minDistanse = temp;
                        nearestElevation = baseObject.BoundRectangle.TopCenter;
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}