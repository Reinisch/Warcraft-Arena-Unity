using Bolt;
using UdpKit;
using UnityEngine;

namespace Core
{
    public class ClientConnectionToken : IProtocolToken
    {
        public string Name { get; private set; }
        public string UnityId { get; private set; }

        public ClientConnectionToken()
        {
            Name = "Player";
            UnityId = SystemInfo.deviceUniqueIdentifier;
        }

        public ClientConnectionToken(string name)
        {
            Name = name;
            UnityId = SystemInfo.deviceUniqueIdentifier;
        }

        public void Read(UdpPacket packet)
        {
            Name = packet.ReadString();
            UnityId = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(Name);
            packet.WriteString(UnityId);
        }
    }
}
