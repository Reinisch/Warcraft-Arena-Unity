namespace Core
{
    public class PhotonPacketContainer
    {
        public WorldPacket Packet { get; private set; }

        public PhotonPacketContainer(WorldPacket packet)
        {
            Packet = packet;
        }
    }
}
