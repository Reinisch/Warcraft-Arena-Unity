using System;
using Core;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Payload a client sends when joining a session (identity + preferred class + version gate).
    /// (Bolt: ClientConnectionToken, an IProtocolToken.)
    /// </summary>
    public sealed class ClientConnectionToken : INetSerializable
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string UnityId { get; private set; }
        public ClassType PreferredClass { get; set; }
        public bool IsValid { get; private set; } = true;

        public ClientConnectionToken()
        {
            Name = "Player";
            UnityId = SystemInfo.deviceUniqueIdentifier;
        }

        public void Write(INetWriter writer)
        {
            writer.WriteString(Name);
            writer.WriteString(UnityId);
            writer.WriteString(Version);
            writer.WriteInt((int)PreferredClass);
        }

        public void Read(INetReader reader)
        {
            try
            {
                Name = reader.ReadString();
                UnityId = reader.ReadString();
                Version = reader.ReadString();
                PreferredClass = (ClassType)reader.ReadInt();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }
    }
}
