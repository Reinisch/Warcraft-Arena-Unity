using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BasicRpgEngine
{
    public static class InputHandler
    {
        static KeyboardState keyboardState = Keyboard.GetState();
        static KeyboardState lastKeyboardState = Keyboard.GetState();
        static MouseState mouseState = Mouse.GetState();
        static MouseState lastMouseState = Mouse.GetState();

        public static MouseState MouseState
        {
            get { return mouseState; }
        }
        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }
        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }
        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        public static void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
        public static void Flush()
        {
            lastKeyboardState = keyboardState;
        }
        public static bool NoModifier()
        {
            return !KeyDown(Keys.LeftAlt) && !KeyDown(Keys.LeftControl) && !KeyDown(Keys.LeftShift);
        }
        public static bool LeftMouseClick()
        {
            return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
        }
        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }
        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
    }
}
