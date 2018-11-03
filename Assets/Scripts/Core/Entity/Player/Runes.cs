using System;
using System.Collections.Generic;

namespace Core
{
    public class Runes
    {
        /// <summary>
        /// Mask of available runes.
        /// </summary>
        public byte RuneState { get; set; }

        public List<byte> CooldownOrder { get; set; }
        public uint[] Cooldown { get; set; }


        public Runes()
        {
            Cooldown = new uint[PlayerHelper.MaxRunes];
            CooldownOrder = new List<byte>();
        }

        public void SetRuneState(byte index, bool set = true) { throw new NotImplementedException(); }
    }
}