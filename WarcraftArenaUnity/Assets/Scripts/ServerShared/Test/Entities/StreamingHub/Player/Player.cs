using MessagePack;
using UnityEngine;

namespace ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player
{
    [MessagePackObject]
    public class Player
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public Vector3 Position { get; set; }
        [Key(2)]
        public Quaternion Rotation { get; set; }
    }
}
