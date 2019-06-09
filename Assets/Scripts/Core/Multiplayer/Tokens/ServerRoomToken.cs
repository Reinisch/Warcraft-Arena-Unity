using Bolt;
using UdpKit;

namespace Core
{
    public class ServerRoomToken : IProtocolToken
    {
        public string Name { get; private set; }
        public string Map { get; private set; }

        public ServerRoomToken()
        {
        }

        public ServerRoomToken(string name, string map)
        {
            Name = name;
            Map = map;
        }

        public void Read(UdpPacket packet)
        {
            Name = packet.ReadString();
            Map = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(Name);
            packet.WriteString(Map);
        }
    }
}
