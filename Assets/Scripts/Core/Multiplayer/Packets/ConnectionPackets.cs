using ExitGames.Client.Photon;
using JetBrains.Annotations;

namespace Core.Packets
{
    [UsedImplicitly]
    public class ConnectionPacket : ClientPacket
    {
        public override ClientOpCodes OpCode => ClientOpCodes.Connect;

        public string PlayerName { get; set; }
    }
}
