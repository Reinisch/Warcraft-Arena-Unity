namespace Core
{
    public class CameraEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Camera;

        public uint Open { get; set; }                      // 0 open, References: Lock_, NoValue = 0
        public uint Camera { get; set; }                    // 1 camera, References: CinematicSequences, NoValue = 0
        public uint EventID { get; set; }                   // 2 eventID, References: GameEvents, NoValue = 0
        public uint OpenTextID { get; set; }                // 3 openTextID, References: BroadcastText, NoValue = 0
        public uint ConditionID1 { get; set; }              // 4 conditionID1, References: PlayerCondition, NoValue = 0

        public override uint LockId => Open;
        public override uint EventScriptId => EventID;
    }
}