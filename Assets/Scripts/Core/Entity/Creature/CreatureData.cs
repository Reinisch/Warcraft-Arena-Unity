namespace Core
{
    public class CreatureData
    {
        public uint Id;                 // entry in creature_template
        public ushort MapId;
        public uint PhaseMask;
        public uint DisplayId;
        public byte EquipmentId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float Orientation;
        public uint SpawntimeSecs;
        public float SpawnDist;
        public uint CurrentWaypoint;
        public uint CurHealth;
        public uint CurMana;
        public byte MovementType;
        public uint SpawnMask;
        public ulong Npcflag;
        public uint UnitFlags;          // enum UnitFlags mask values
        public uint DynamicFlags;
        public uint PhaseId;
        public uint PhaseGroup;
        public bool DbData;

        public CreatureData()
        {
            Id = 0;
            MapId = 0;
            PhaseMask = 0;
            DisplayId = 0;
            EquipmentId = 0;
            PosX = 0.0f;
            PosY = 0.0f;
            PosZ = 0.0f;
            Orientation = 0.0f;
            SpawntimeSecs = 0;
            SpawnDist = 0.0f;
            CurrentWaypoint = 0;
            CurHealth = 0;
            CurMana = 0;
            MovementType = 0;
            SpawnMask = 0;
            Npcflag = 0;
            UnitFlags = 0;
            DynamicFlags = 0;
            PhaseId = 0;
            PhaseGroup = 0;
            DbData = true;
        }
    }

    public class EquipmentInfo
    {
        public uint[] ItemEntry; // UnitHelper.MaxEquippedItems
    }

    public class CreatureModelInfo
    {
        public float BoundingRadius;
        public float CombatReach;
        public Gender Gender;
        public uint DisplayIdOtherGender;
        public bool IsTrigger;
    }
}