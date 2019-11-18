using System;
using System.Collections.Generic;
using System.Text;
using ElleFramework.Database.MVC;

namespace ElleRealTime.Shared.DBEntities.PlayersInfo
{
    public class PlayerInfo : View
    {
        public int AccountID { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
    }
}
