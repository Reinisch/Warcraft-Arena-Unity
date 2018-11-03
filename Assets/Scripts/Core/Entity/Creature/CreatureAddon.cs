using System.Collections.Generic;

namespace Core
{
    public class CreatureAddon
    {
        public uint PathID;
        public uint Mount;
        public uint Bytes1;
        public uint Bytes2;
        public uint Emote;
        public ushort AIAnimKit;
        public ushort MovementAnimKit;
        public ushort MeleeAnimKit;
        public List<uint> Auras;
    }
}