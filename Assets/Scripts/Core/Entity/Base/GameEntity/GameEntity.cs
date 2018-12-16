using System;
using System.Collections.Generic;

namespace Core
{
    public class GameEntity : WorldEntity, IGridEntity<GameEntity>
    {
        public override EntityType EntityType => EntityType.GameEntity;
        public override bool AutoScoped => true;

        public GridReference<GameEntity> GridRef { get; private set; }

        public int Usetimes { get; protected set; }
        public uint SpellId { get; protected set; }
        public long RespawnTime { get; protected set; }
        public uint RespawnDelayTime { get; protected set; }
        public bool SpawnedByDefault { get; protected set; }
        public long CooldownTime { get; protected set; }

        public List<ulong> UniqueUsers { get; protected set; }

        public Guid SpawnId { get; protected set; }
        public GameEntityAI AI { get; private set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<GameEntity> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public override void DoUpdate(int timeDelta)
        {

        }
    }
}