using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BasicRpgEngine.Controls
{
    public class ListBox : Control
    {
        public event EventHandler SelectionChanged;
        public event EventHandler Enter;
        public event EventHandler Leave;

        List<string> items = new List<string>();
        Color selectedColor = Color.Red;
        int startItem;
        int lineCount;
        Texture2D image;
        Texture2D cursor;
        int selectedItem;

        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }
        public int SelectedIndex
        {
            get { return selectedItem; }
            set { selectedItem = (int)MathHelper.Clamp(value, 0f, items.Count); }
        }
        public string SelectedItem
        {
            get { return items[selectedItem]; }
        }
        public List<string> Items
        {
            get { return items; }
        }
        public override bool HasFocus
        {
            get
            {
                return hasFocus;
            }
            set
            {
                hasFocus = value;
                if (hasFocus)
                    OnEnter(null);
                else
                    OnLeave(null);
            }
        }

        public ListBox(Texture2D background, Texture2D cursor)
            : base()
        {
            hasFocus = false;
            tabStop = false;

            this.image = background;
            this.cursor = cursor;
            this.Size = new Vector2(image.Width, image.Height);

            lineCount = image.Height / SpriteFont.LineSpacing;
            startItem = 0;
            Color = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {}
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Position, Color.White);

            for (int i = 0; i < lineCount; i++)
            {
                if (startItem + i >= items.Count)
                    break;

                if (startItem + i == selectedItem)
                {
                    spriteBatch.DrawString(SpriteFont, items[startItem + i],
                        new Vector2(Position.X, Position.Y + i * SpriteFont.LineSpacing),
                        SelectedColor);
                    spriteBatch.Draw(cursor,
                        new Vector2(Position.X - (cursor.Width + 2),
                            Position.Y + i * SpriteFont.LineSpacing + 5),
                            Color.White);
                }
                else
                    spriteBatch.DrawString(SpriteFont, items[startItem + i],
                        new Vector2(Position.X, 2 + Position.Y + i * SpriteFont.LineSpacing),
                        Color);
            }

        }
        public override void HandleInput()
        {
            if (!HasFocus)
                return;

            if (InputHandler.KeyReleased(Keys.Down))
            {
                if (selectedItem < items.Count - 1)
                {
                    selectedItem++;
                    if (selectedItem >= startItem + lineCount)
                        startItem = selectedItem - lineCount + 1;
                    OnSelectionChanged(null);
                }
            }
            else if (InputHandler.KeyReleased(Keys.Up))
            {
                if (selectedItem > 0)
                {
                    selectedItem--;
                    if (selectedItem < startItem)
                        startItem = selectedItem;
                    OnSelectionChanged(null);
                }
            }

            if (InputHandler.KeyReleased(Keys.Enter))
            {
                HasFocus = false;
                OnSelected(null);
            }

            if (InputHandler.KeyReleased(Keys.Escape))
            {
                HasFocus = false;
            }
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }
        protected virtual void OnEnter(EventArgs e)
        {
            if (Enter != null)
                Enter(this, e);
        }
        protected virtual void OnLeave(EventArgs e)
        {
            if (Leave != null)
                Leave(this, e);
        }
    }
}