using UdpKit;

namespace Core
{
    public abstract partial class Unit
    {
        public new class CreateToken : WorldEntity.CreateToken
        {
            public SpellPowerType DisplayPowerType { private get; set; }
            public DeathState DeathState { private get; set; }
            public EmoteType EmoteType { private get; set; }
            public ClassType ClassType { private get; set; }
            public bool FreeForAll { private get; set; }
            public int FactionId { private get; set; }
            public int ModelId { private get; set; }
            public int OriginalModelId { private get; set; }
            public float Scale { private get; set; } = 1.0f;

            public int OriginalAIInfoId { get; set; }

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                DisplayPowerType = (SpellPowerType) packet.ReadInt();
                DeathState = (DeathState) packet.ReadInt();
                EmoteType = (EmoteType) packet.ReadInt();
                ClassType = (ClassType) packet.ReadInt();
                FactionId = packet.ReadInt();
                ModelId = packet.ReadInt();
                OriginalModelId = packet.ReadInt();
                OriginalAIInfoId = packet.ReadInt();
                FreeForAll = packet.ReadBool();
                Scale = packet.ReadFloat();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteInt((int)DisplayPowerType);
                packet.WriteInt((int) DeathState);
                packet.WriteInt((int) EmoteType);
                packet.WriteInt((int) ClassType);
                packet.WriteInt(FactionId);
                packet.WriteInt(ModelId);
                packet.WriteInt(OriginalModelId);
                packet.WriteInt(OriginalAIInfoId);
                packet.WriteBool(FreeForAll);
                packet.WriteFloat(Scale);
            }

            protected void Attached(Unit unit)
            {
                unit.DisplayPowerType = DisplayPowerType;
                unit.DeathState = DeathState;
                unit.EmoteType = EmoteType;
                unit.ClassType = ClassType;
                unit.Faction = unit.Balance.FactionsById[FactionId];
                unit.FreeForAll = FreeForAll;
                unit.ModelId = ModelId;
                unit.OriginalModelId = OriginalModelId;
                unit.Scale = Scale;
            }
        }
    }
}
