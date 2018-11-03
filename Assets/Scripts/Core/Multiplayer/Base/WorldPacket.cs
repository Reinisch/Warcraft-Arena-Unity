using System;
using System.Text;
using Core.Packets;
using ExitGames.Client.Photon;

namespace Core
{
    public abstract class WorldPacket
    {
        public abstract int OpCodeIndex { get; }

        /// <summary>
        /// Method for custom Photon serialization.
        /// </summary>
        public abstract void Serialize(StreamBuffer outStream, Protocol16 photonProtocol);

        /// <summary>
        /// Method for custom Photon deserialization.
        /// </summary>
        public abstract void Deserialize(StreamBuffer inStream, Protocol16 photonProtocol);
    }

    public abstract class ClientPacket : WorldPacket, IPacket<ClientOpCodes>
    {
        public abstract ClientOpCodes OpCode { get; }
        public override int OpCodeIndex => (int) OpCode;

    }

    public abstract class ServerPacket : WorldPacket, IPacket<ServerOpCodes>
    {
        public abstract ServerOpCodes OpCode { get; }
        public override int OpCodeIndex => (int)OpCode;
    }
}