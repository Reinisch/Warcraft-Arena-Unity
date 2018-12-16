using System;

namespace Core
{
    public class Corpse : WorldEntity, IGridEntity<Corpse>
    {
        public override EntityType EntityType => EntityType.Corpse;
        public override bool AutoScoped => true;

        public GridReference<Corpse> GridRef { get; private set; }
        public CorpseType CorpseType { get; private set; }

        public bool LootForBody { get; set; }
        public long GhostTime { get; private set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<Corpse> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public bool Create(Guid guidlow, Map map) { throw new NotImplementedException(); }
        public bool Create(Guid guidlow, Player owner) { throw new NotImplementedException(); }

        public bool IsExpired(long t) { throw new NotImplementedException(); }
        public void ResetGhostTime() { GhostTime = TimeHelper.NowInMilliseconds; }
    }
}