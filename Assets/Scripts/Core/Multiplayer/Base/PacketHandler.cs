using System;
using JetBrains.Annotations;

namespace Core
{
    public interface IPacket<out T>
    {
        T OpCode { get; }
    }

    public interface IPacketHandler<T>
    {
        void Invoke(WorldSession session, IPacket<T> packet);
    }

    public class PacketHandler<TSession, TPacketOpCode, TPacket> : IPacketHandler<TPacketOpCode> where TSession : WorldSession where TPacket : IPacket<TPacketOpCode>
    {
        private readonly Action<TSession, TPacket> handler;

        [UsedImplicitly]
        public PacketHandler(Action<TSession, TPacket> handler)
        {
            this.handler = handler;
        }

        public void Invoke(WorldSession session, IPacket<TPacketOpCode> packet)
        {
            handler((TSession)session, (TPacket)packet);
        }
    }
}
