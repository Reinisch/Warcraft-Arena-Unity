namespace Core
{
    public sealed partial class Player
    {
        public override UnitSnapshot CaptureState()
        {
            UnitSnapshot snapshot = base.CaptureState();
            snapshot.Kind = UnitSnapshotKind.Player;
            return snapshot;
        }
    }
}
