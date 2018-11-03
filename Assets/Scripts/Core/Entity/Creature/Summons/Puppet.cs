namespace Core
{
    public class Puppet : Minion
    {
        public Puppet(Unit owner) : base(owner, false)
        {
        
        }

        public override void InitStats(uint duration) { }
        public override void InitSummon() { }
        public override void DoUpdate(uint time) { }
        public override void RemoveFromWorld() { }
    }
}