using System;

namespace Core
{
    public class TempSummon : Creature
    {
        public TempSummonType SummonType { get; set; }
        public uint Timer { get; private set; }
        public uint Lifetime { get; private set; }
        public Guid SummonerGuid { get; private set; }

        public Unit Summoner => null;
        public Creature SummonerCreatureBase => null;

        public TempSummon(Unit owner, bool isWorldObject)
        {
        }

        public virtual void InitStats(uint lifetime) { }
        public virtual void InitSummon() { }
        public virtual void UnSummon(uint msTime = 0) { }

        public override void RemoveFromWorld() { }
        public override void DoUpdate(uint time) { }
    }
}