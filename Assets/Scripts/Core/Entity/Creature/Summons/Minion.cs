namespace Core
{
    public class Minion : TempSummon
    {
        public new Unit Owner { get; private set; }
        public float FollowAngle { get; set; }

        public bool IsPetGhoul => Entry == 26125;
        // Ghoul may be guardian or pet
        public bool IsSpiritWolf => Entry == 29264;
        // Spirit wolf from feral spirits
        public bool IsGuardianPet => false;

        public Minion(Unit owner, bool isWorldObject) : base(owner, isWorldObject)
        {
        
        }

        public override void InitStats(uint duration) { }
        public override void RemoveFromWorld() { }
    }
}