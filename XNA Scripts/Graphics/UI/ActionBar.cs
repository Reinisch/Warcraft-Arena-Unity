using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

using BasicRpgEngine.Characters;
using BasicRpgEngine;

namespace BasicRpgEngine
{
    public class ActionBar
    {
        private int iconSize;
        private int buttonCount;
        private int buttonInterval;
        private SpriteFont cooldownFont;

        public NetworkPlayerInterface NetPlayerInterfaceRef
        { get; private set; }
        public ActionBarButton[] Buttons 
        { get; private set; }
        public Point Position
        { get; set; }
        public int ButtonCount
        {
            get { return buttonCount; }
            set
            {
                if (value < 1)
                    buttonCount = 0;
                else if (value > 16)
                    buttonCount = 16;
                else
                    buttonCount = value;
            }
        }
        public int ButtonInterval
        {
            get { return buttonInterval; }
            set
            {
                if (value < 1)
                    buttonInterval = 0;
                else if (value > 10)
                    buttonInterval = 10;
                else
                    buttonInterval = value;
            }
        }
        public int IconSize
        { get { return iconSize; } }

        public ActionBar(Game game,NetworkPlayerInterface interfaceRef, Point position)
        {
            Buttons = new ActionBarButton[14];
            iconSize = 48;
            buttonCount = 14;
            NetPlayerInterfaceRef = interfaceRef;
            Position = position;
            ButtonInterval = 1;
            cooldownFont = game.Content.Load<SpriteFont>(@"Fonts\CooldownFont");
        }

        public void Update(GameTime gameTime, World WorldRef)
        {
            for (int i = 0; i < ButtonCount; i++)
                if (Buttons[i].ButtonSpellRef != null)
                    if(Buttons[i].ButtonPress())
                        WorldRef.ApplySpell(NetPlayerInterfaceRef.PlayerRef, Buttons[i].ButtonSpellRef,
                            NetPlayerInterfaceRef.PlayerRef.Character.Target, false, false);
        }
        public void UpdateHost(GameTime gameTime, World WorldRef, NetworkSession networkSession)
        {
            for (int i = 0; i < ButtonCount; i++)
            {
                if (Buttons[i].ButtonSpellRef != null)
                    if (Buttons[i].ButtonPress())
                        if (WorldRef.ApplySpell(NetPlayerInterfaceRef.PlayerRef, Buttons[i].ButtonSpellRef,
                            NetPlayerInterfaceRef.PlayerRef.Character.Target, false, false))
                        {
                            LocalNetworkGamer server = (LocalNetworkGamer)networkSession.Host;

                            foreach (NetworkGamer negamer in networkSession.RemoteGamers)
                            {
                                PacketWriter packetWriter = new PacketWriter();
                                NetworkPlayer everyPlayer = negamer.Tag as NetworkPlayer;
                                packetWriter.Write('S');
                                packetWriter.Write(NetPlayerInterfaceRef.PlayerRef.ID);
                                packetWriter.Write(Buttons[i].ButtonSpellRef.SpellDataId);

                                if (NetPlayerInterfaceRef.PlayerRef.Character.Target == null)
                                    packetWriter.Write(0);
                                else
                                    packetWriter.Write(NetPlayerInterfaceRef.PlayerRef.Character.Target.ID);

                                server.SendData(packetWriter, SendDataOptions.InOrder);
                                packetWriter.Flush();
                            }
                        }
            }
        }
        public bool UpdateNetwork(GameTime gameTime, ref PacketWriter packetWriter)
        {
            bool result = false;
            for (int i = 0; i < ButtonCount; i++)
            {
                if (Buttons[i].ButtonSpellRef != null)
                    if (Buttons[i].ButtonPress())
                    {
                        result = true;
                        packetWriter.Write(Buttons[i].ButtonSpellRef.SpellDataId);
                        if (NetPlayerInterfaceRef.PlayerRef.Character.Target == null)
                            packetWriter.Write(0);
                        else
                            packetWriter.Write(NetPlayerInterfaceRef.PlayerRef.Character.Target.ID);
                    }
            }
            return result;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D iconToDraw;
            int iconSize = IconSize;
            string strCooldown;
            for (int i = 0; i < buttonCount; i++)
            {
                if (Buttons[i].ButtonSpellRef != null)
                {
                    if (NetPlayerInterfaceRef.Icons.TryGetValue(Buttons[i].ButtonSpellRef.Name, out iconToDraw))
                    {
                        if (!Buttons[i].ButtonSpellRef.SpellCooldown.NoCooldown)
                        {
                            spriteBatch.Draw(iconToDraw, new Rectangle(Position.X + i * (iconSize + buttonInterval), Position.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.DarkGray);
                            if (Buttons[i].ButtonSpellRef.SpellCooldown.TimeLeft.TotalSeconds < 1)
                            {
                                strCooldown = "0." + (Buttons[i].ButtonSpellRef.SpellCooldown.TimeLeft.Milliseconds / 100).ToString();
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X + 2 + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + 2 + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X - 2 + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown)/2, Color.Yellow);
                            }
                            else
                            {
                                strCooldown = (Buttons[i].ButtonSpellRef.SpellCooldown.TimeLeft.Seconds).ToString();
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X + 2 + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + 2 + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X - 2 + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(cooldownFont, strCooldown, new Vector2(Position.X + i * (iconSize + buttonInterval) + iconSize / 2, Position.Y + iconSize / 2 - 2) - cooldownFont.MeasureString(strCooldown) / 2, Color.Yellow);
                            }
                        }
                        else
                            spriteBatch.Draw(iconToDraw, new Rectangle(Position.X + i * (iconSize + buttonInterval), Position.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);
                    }
                    else if (NetPlayerInterfaceRef.Icons.TryGetValue("Generic", out iconToDraw))
                        spriteBatch.Draw(iconToDraw, new Rectangle(Position.X + i * (iconSize + buttonInterval), Position.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);

                    spriteBatch.DrawString(NetPlayerInterfaceRef.HotkeyFont, Buttons[i].ToString(), new Vector2(Position.X + 28 + i * (iconSize + buttonInterval), Position.Y), Color.Black, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(NetPlayerInterfaceRef.HotkeyFont, Buttons[i].ToString(), new Vector2(Position.X + 24 + i * (iconSize + buttonInterval), Position.Y), Color.Black, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(NetPlayerInterfaceRef.HotkeyFont, Buttons[i].ToString(), new Vector2(Position.X + 26 + i * (iconSize + buttonInterval), Position.Y - 4), Color.Black, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(NetPlayerInterfaceRef.HotkeyFont, Buttons[i].ToString(), new Vector2(Position.X + 26 + i * (iconSize + buttonInterval), Position.Y), Color.Black, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(NetPlayerInterfaceRef.HotkeyFont, Buttons[i].ToString(), new Vector2(Position.X + 26 + i * (iconSize + buttonInterval), Position.Y - 2), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                }
            }
        }
    }
}