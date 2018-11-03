using ExitGames.Client.Photon;
using JetBrains.Annotations;

namespace Core.Packets
{
    [UsedImplicitly]
    public class ConnectionPacket : ClientPacket
    {
        public override ClientOpCodes OpCode => ClientOpCodes.Connect;

        public string PlayerName { get; set; }

        public override void Serialize(StreamBuffer outStream, Protocol16 photonProtocol)
        {
            photonProtocol.SerializeString(outStream, PlayerName, false);
        }

        public override void Deserialize(StreamBuffer inStream, Protocol16 photonProtocol)
        {
            PlayerName = (string)photonProtocol.Deserialize(inStream, (byte)Protocol16.GpType.String);
        }
    }
}
