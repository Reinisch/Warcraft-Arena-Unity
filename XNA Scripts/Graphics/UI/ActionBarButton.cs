using System;

using Microsoft.Xna.Framework.Input;
using BasicRpgEngine.Spells;


namespace BasicRpgEngine
{
    public class ActionBarButton
    {
        public Spell ButtonSpellRef { get; private set; }
        public Keys Mod { get; private set; }
        public Keys Key { get; private set; }

        public ActionBarButton(Spell spellRef, Keys key, Keys mod)
        {
            ButtonSpellRef = spellRef;
            if (!(mod == Keys.LeftControl || mod == Keys.LeftAlt || mod == Keys.LeftShift))
                mod = Keys.None;

            Mod = mod;
            Key = key;
        }

        public bool ButtonPress()
        {
            if (Mod != Keys.None)
                return InputHandler.KeyDown(Mod) && InputHandler.KeyPressed(Key);
            else
                return InputHandler.NoModifier() && InputHandler.KeyPressed(Key);
        }
        public override string ToString()
        {
            string result;
            switch (Mod)
            {
                case Keys.LeftControl:
                    result = "C";
                    break;
                case Keys.LeftShift:
                    result = "S";
                    break;
                case Keys.LeftAlt:
                    result = "A";
                    break;
                default:
                    result = "";
                    break;
            }
            switch (Key)
            {
                case Keys.D1:
                    result += "1";
                    break;
                case Keys.D2:
                    result += "2";
                    break;
                case Keys.D3:
                    result += "3";
                    break;
                case Keys.D4:
                    result += "4";
                    break;
                case Keys.D5:
                    result += "5";
                    break;
                case Keys.D6:
                    result += "6";
                    break;
                case Keys.D7:
                    result += "7";
                    break;
                case Keys.D8:
                    result += "8";
                    break;
                case Keys.D9:
                    result += "9";
                    break;
                case Keys.D0:
                    result += "0";
                    break;
                default:
                    result += Key.ToString();
                    break;
            }
            return result;
        }
    }
}