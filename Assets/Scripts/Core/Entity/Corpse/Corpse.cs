using System;

namespace Core
{
    public class Corpse : WorldEntity, IGridEntity<Corpse>
    {
        public GridReference<Corpse> GridRef { get; private set; }
        public CorpseType CorpseType { get; private set; }

        public Guid OwnerGUID => GetGuidValue(EntityFields.CorpseOwner);
        public Player LootRecipient { get; set; }

        public bool LootForBody { get; set; }
        public long GhostTime { get; private set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<Corpse> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public override void AddToWorld() { throw new NotImplementedException(); }
        public override void RemoveFromWorld() { throw new NotImplementedException(); }

        public bool Create(Guid guidlow, Map map) { throw new NotImplementedException(); }
        public bool Create(Guid guidlow, Player owner) { throw new NotImplementedException(); }

        public bool IsExpired(long t) { throw new NotImplementedException(); }
        public void ResetGhostTime() { GhostTime = TimeHelper.NowInMilliseconds; }
    }
}