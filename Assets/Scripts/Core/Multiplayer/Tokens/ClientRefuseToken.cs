using Bolt;
using UdpKit;

namespace Core
{
    public class ClientRefuseToken : IProtocolToken
    {
        public ConnectRefusedReason Reason { get; private set; }

        public ClientRefuseToken()
        {
            Reason = ConnectRefusedReason.None;
        }

        public ClientRefuseToken(ConnectRefusedReason reason)
        {
            Reason = reason;
        }

        public void Read(UdpPacket packet)
        {
            Reason = (ConnectRefusedReason)packet.ReadInt();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteInt((int)Reason);
        }
    }
}
