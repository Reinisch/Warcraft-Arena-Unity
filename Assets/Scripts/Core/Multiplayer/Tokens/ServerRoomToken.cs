using Bolt;
using UdpKit;

namespace Core
{
    public class ServerRoomToken : IProtocolToken
    {
        public string LocalPlayerName { get; private set; }
        public string Name { get; private set; }
        public string Map { get; private set; }
        public string Version { get; set; }

        public ServerRoomToken()
        {
            LocalPlayerName = "Server Player";
            Name = "Default Server";
            Map = "Lordaeron";
        }

        public ServerRoomToken(string name, string localPlayerName, string map)
        {
            LocalPlayerName = localPlayerName;
            Name = name;
            Map = map;
        }

        public void Read(UdpPacket packet)
        {
            LocalPlayerName = packet.ReadString();
            Name = packet.ReadString();
            Map = packet.ReadString();
            Version = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(LocalPlayerName);
            packet.WriteString(Name);
            packet.WriteString(Map);
            packet.WriteString(Version);
        }
    }
}
