namespace Net
{
    public sealed class ServerRoomToken : INetSerializable
    {
        public string LocalPlayerName { get; private set; }
        public string Name { get; private set; }
        public string Map { get; private set; }
        public string Version { get; set; }
        public int Scenario { get; set; }
        public int TeamSize { get; set; }

        public ServerRoomToken()
        {
            LocalPlayerName = "Server Player";
            Name = "Default Server";
            Map = "Lordaeron";
        }

        public ServerRoomToken(string name, string localPlayerName, string map, int scenario)
        {
            LocalPlayerName = localPlayerName;
            Name = name;
            Map = map;
            Scenario = scenario;
        }

        public void Write(INetWriter writer)
        {
            writer.WriteString(LocalPlayerName);
            writer.WriteString(Name);
            writer.WriteString(Map);
            writer.WriteString(Version);
            writer.WriteInt(Scenario);
            writer.WriteInt(TeamSize);
        }

        public void Read(INetReader reader)
        {
            LocalPlayerName = reader.ReadString();
            Name = reader.ReadString();
            Map = reader.ReadString();
            Version = reader.ReadString();
            Scenario = reader.ReadInt();
            TeamSize = reader.ReadInt();
        }
    }
}
