using System.Collections.Generic;

namespace Core
{
    public class PlayerInfo
    {
        public uint MapId { get; set; }
        public uint AreaId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Orientation { get; set; }
        public ushort DisplayIdM { get; set; }
        public ushort DisplayIdF { get; set; }

        public List<uint> CustomSpells { get; set; }
        public List<uint> CastSpells { get; set; }
        public List<PlayerCreateInfoItem> Item { get; set; }
        public List<PlayerCreateInfoAction> Action { get; set; }
        public PlayerLevelInfo LevelInfo { get; set; }

        public PlayerInfo()
        {
            MapId = 0;
            AreaId = 0;
            PositionX = 0.0f;
            PositionY = 0.0f;
            PositionZ = 0.0f;
            Orientation = 0.0f;
            DisplayIdM = 0;
            DisplayIdF = 0;

            Item = new List<PlayerCreateInfoItem>();
            CustomSpells = new List<uint>();
            CastSpells = new List<uint>();
            Action = new List<PlayerCreateInfoAction>();
            LevelInfo = new PlayerLevelInfo();
        }
    }
}