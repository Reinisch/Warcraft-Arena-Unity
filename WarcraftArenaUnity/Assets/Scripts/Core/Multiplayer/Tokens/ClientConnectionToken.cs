using System;
using Bolt;
using UdpKit;
using UnityEngine;

namespace Core
{
    public class ClientConnectionToken : IProtocolToken
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string UnityId { get; private set; }
        public ClassType PrefferedClass { get; set; }
        public bool IsValid { get; private set; } = true;

        public ClientConnectionToken()
        {
            Name = "Player";
            UnityId = SystemInfo.deviceUniqueIdentifier;
        }

        public void Read(UdpPacket packet)
        {
            try
            {
                Name = packet.ReadString();
                UnityId = packet.ReadString();
                Version = packet.ReadString();
                PrefferedClass = (ClassType) packet.ReadInt();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(Name);
            packet.WriteString(UnityId);
            packet.WriteString(Version);
            packet.WriteInt((int)PrefferedClass);
        }
    }
}
