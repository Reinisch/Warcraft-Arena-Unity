using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BasicRpgEngine.Physics
{
    [Serializable]
    public class MapPart
    {
        public Texture2D BackgroundImage
        { get; private set; }
        public Color Color
        { get; set; }
        public int Width
        {
            get { return BackgroundImage.Width; }
        }
        public int Height
        {
            get { return BackgroundImage.Height; }
        }

        public MapPart(Texture2D backgroundImage, Color color)
        {
            BackgroundImage = backgroundImage;
            Color = color;
        }
    }
}