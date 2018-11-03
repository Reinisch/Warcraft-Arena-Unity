using System;

namespace Core
{
    public class FriendInfo
    {
        public Guid WowAccountGuid { get; set; }
        public FriendStatus Status { get; set; }
        public byte Flags { get; set; }
        public uint Area { get; set; }
        public byte Level { get; set; }
        public byte Class { get; set; }
        public string Note { get; set; }


        public FriendInfo()
        {
            Status = FriendStatus.Offline;
            Note = "";
        }

        public FriendInfo(Guid accountGuid, byte flags, string note)
        {
            Status = FriendStatus.Offline;
            WowAccountGuid = accountGuid;
            Flags = flags;
            Note = note;
        }
    }
}