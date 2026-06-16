using UnityEngine;

namespace Core
{
    public enum UnitSnapshotKind
    {
        Creature,
        Player,
    }

    /// <summary>
    /// Framework-neutral snapshot of a Unit's replicable spawn state. Captured on the server via
    /// <see cref="Unit.CaptureState"/> and used by the network layer to recreate the unit on a client.
    /// Core owns what's replicable; adapters just serialize/mirror these fields (this is the concrete
    /// type behind the <c>INetState&lt;T&gt;</c> seam).
    /// </summary>
    public struct UnitSnapshot
    {
        public UnitSnapshotKind Kind;
        public Vector3 Position;
        public Quaternion Rotation;
        public int ModelId;
        public int OriginalModelId;
        public ClassType ClassType;
        public int FactionId;
        public DeathState DeathState;
        public EmoteType EmoteType;
        public UnitVisualEffectFlags VisualEffectFlags;
        public SpellPowerType DisplayPowerType;
        public int DisplayPower;
        public int DisplayPowerMax;
        public float Scale;
        public bool FreeForAll;
        public int OriginalAIInfoId;
        public int CreatureInfoId;
        public string Name;

        /// <summary>
        /// Builds the Core <see cref="Entity.CreateToken"/> to recreate this unit on a client — the
        /// inverse of <see cref="Unit.CaptureState"/>. The map is client-local, so it's passed in.
        /// </summary>
        public Entity.CreateToken ToCreateToken(Map map)
        {
            if (Kind == UnitSnapshotKind.Player)
            {
                return new Player.CreateToken
                {
                    Position = Position,
                    Rotation = Rotation,
                    Map = map,
                    ModelId = ModelId,
                    OriginalModelId = OriginalModelId,
                    ClassType = ClassType,
                    FactionId = FactionId,
                    DeathState = DeathState,
                    EmoteType = EmoteType,
                    VisualEffectFlags = VisualEffectFlags,
                    DisplayPowerType = DisplayPowerType,
                    DisplayPower = DisplayPower,
                    DisplayPowerMax = DisplayPowerMax,
                    Scale = Scale,
                    FreeForAll = FreeForAll,
                    OriginalAIInfoId = OriginalAIInfoId,
                    PlayerName = Name,
                };
            }

            return new Creature.CreateToken
            {
                Position = Position,
                Rotation = Rotation,
                Map = map,
                ModelId = ModelId,
                OriginalModelId = OriginalModelId,
                ClassType = ClassType,
                FactionId = FactionId,
                DeathState = DeathState,
                EmoteType = EmoteType,
                VisualEffectFlags = VisualEffectFlags,
                DisplayPowerType = DisplayPowerType,
                DisplayPower = DisplayPower,
                DisplayPowerMax = DisplayPowerMax,
                Scale = Scale,
                FreeForAll = FreeForAll,
                OriginalAIInfoId = OriginalAIInfoId,
                CustomName = Name,
                CreatureInfoId = CreatureInfoId,
            };
        }
    }
}
