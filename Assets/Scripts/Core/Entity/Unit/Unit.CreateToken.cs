using UdpKit;

namespace Core
{
    public abstract partial class Unit
    {
        public new class CreateToken : WorldEntity.CreateToken
        {
            public UnitVisualEffectFlags VisualEffectFlags { private get; set; }
            public SpellPowerType DisplayPowerType { private get; set; }
            public DeathState DeathState { private get; set; }
            public EmoteType EmoteType { private get; set; }
            public ClassType ClassType { private get; set; }
            public bool FreeForAll { private get; set; }
            public int DisplayPower { private get; set; }
            public int DisplayPowerMax { private get; set; }
            public int FactionId { private get; set; }
            public int ModelId { private get; set; }
            public int OriginalModelId { private get; set; }
            public int OriginalAIInfoId { get; set; }
            public float Scale { private get; set; } = 1.0f;

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                VisualEffectFlags = (UnitVisualEffectFlags) packet.ReadInt();
                DisplayPowerType = (SpellPowerType) packet.ReadInt();
                DeathState = (DeathState) packet.ReadInt();
                EmoteType = (EmoteType) packet.ReadInt();
                ClassType = (ClassType) packet.ReadInt();
                FactionId = packet.ReadInt();
                ModelId = packet.ReadInt();
                OriginalModelId = packet.ReadInt();
                OriginalAIInfoId = packet.ReadInt();
                DisplayPower = packet.ReadInt();
                DisplayPowerMax = packet.ReadInt();
                FreeForAll = packet.ReadBool();
                Scale = packet.ReadFloat();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteInt((int) VisualEffectFlags);
                packet.WriteInt((int) DisplayPowerType);
                packet.WriteInt((int) DeathState);
                packet.WriteInt((int) EmoteType);
                packet.WriteInt((int) ClassType);
                packet.WriteInt(FactionId);
                packet.WriteInt(ModelId);
                packet.WriteInt(OriginalModelId);
                packet.WriteInt(OriginalAIInfoId);
                packet.WriteInt(DisplayPower);
                packet.WriteInt(DisplayPowerMax);
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
                unit.Attributes.SetMaxPower(DisplayPowerType, DisplayPowerMax);
                unit.Attributes.SetPower(DisplayPowerType, DisplayPower);
                unit.Attributes.VisualEffectFlags = VisualEffectFlags;
            }
        }
    }
}
