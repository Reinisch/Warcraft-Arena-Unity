namespace Core
{
    public class GameEntityData
    {
        public uint Id { get; set; }                // entry in gamobject_template
        public ushort MapId { get; set; }
        public uint PhaseMask { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Orientation { get; set; }
        public float Rotation0 { get; set; }
        public float Rotation1 { get; set; }
        public float Rotation2 { get; set; }
        public float Rotation3 { get; set; }
        public int SpawntimeSecs { get; set; }
        public uint AnimProgress { get; set; }
        public GoState GoState { get; set; }
        public uint SpawnMask { get; set; }
        public byte ArtKit { get; set; }
        public uint PhaseId { get; set; }
        public uint PhaseGroup { get; set; }
        public bool DbData { get; set; }

        public GameEntityData()
        {
            Id = 0;
            MapId = 0;
            PhaseMask = 0;
            PosX = 0.0f;
            PosY = 0.0f;
            PosZ = 0.0f;
            Orientation = 0.0f;
            Rotation0 = 0.0f;
            Rotation1 = 0.0f;
            Rotation2 = 0.0f;
            Rotation3 = 0.0f;
            SpawntimeSecs = 0;
            AnimProgress = 0;
            GoState = GoState.Active;
            SpawnMask = 0;
            ArtKit = 0;
            PhaseId = 0;
            PhaseGroup = 0;
            DbData = true;
        }
    }
}