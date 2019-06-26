using System.Collections.Generic;
using Bolt;
using UdpKit;

namespace Core
{
    public sealed class SpellProcessingToken : IProtocolToken
    {
        public readonly List<(ulong, int)> ProcessingEntries = new List<(ulong, int)>();

        public void Read(UdpPacket packet)
        {
            int count = packet.ReadInt();
            for (int i = 0; i < count; i++)
                ProcessingEntries.Add((packet.ReadULong(), packet.ReadInt()));
        }

        public void Write(UdpPacket packet)
        {
            int count = ProcessingEntries.Count;

            packet.WriteInt(count);
            for (int i = 0; i < count; i++)
            {
                packet.WriteULong(ProcessingEntries[i].Item1);
                packet.WriteInt(ProcessingEntries[i].Item2);
            }
        }
    }
}
