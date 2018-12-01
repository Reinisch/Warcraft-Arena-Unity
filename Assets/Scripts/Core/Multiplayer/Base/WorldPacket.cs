using System;
using System.Text;
using Core.Packets;
using ExitGames.Client.Photon;

namespace Core
{
    public abstract class WorldPacket
    {
        public abstract int OpCodeIndex { get; }
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