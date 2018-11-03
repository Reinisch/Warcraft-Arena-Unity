using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class WorldEntity : Entity
    {
        private Map currMap;

        protected string EntityName;
        protected bool IsActive;
        protected bool IsWorldEntity;

        public WorldLocation WorldLocation { get; } = new WorldLocation();
        public MovementInfo MovementInfo { get; } = new MovementInfo();
        public bool IsVisible { get; } = true;

        public float X => WorldLocation.X;
        public float Y => WorldLocation.Y;
        public float Z => WorldLocation.Z;
        public float Orientation => WorldLocation.Orientation;

        public Map Map => currMap;

        public override void Initialize(bool isWorldEntity, Guid guid)
        {
            base.Initialize(isWorldEntity, guid);

            IsWorldEntity = isWorldEntity;
        }

        public override void Deinitialize()
        {
            IsWorldEntity = false;

            base.Deinitialize();
        }

        #region World location methods

        public void WorldRelocate(WorldLocation loc)
        {
            WorldLocation.WorldRelocate(loc);
        }

        public void WorldRelocate(int mapId = GridHelper.MapIdInvalid, float x = 0.0f, float y = 0.0f, float z = 0.0f, float o = 0.0f)
        {
            WorldLocation.WorldRelocate(mapId, x, y, z, o);
        }

        public void Relocate(float x, float y)
        {
            WorldLocation.Relocate(x, y);
        }

        public void Relocate(float x, float y, float z)
        {
            WorldLocation.Relocate(x, y, z);
        }

        public void Relocate(float x, float y, float z, float orientation)
        {
            WorldLocation.Relocate(x, y, z, orientation);
        }

        public void Relocate(Position pos)
        {
            WorldLocation.Relocate(pos);
        }

        public void Relocate(Vector3 pos)
        {
            WorldLocation.Relocate(pos);
        }

        public void RelocateOffset(Position offset)
        {
            WorldLocation.RelocateOffset(offset);
        }

        public void SetOrientation(float orientation)
        {
            WorldLocation.SetOrientation(orientation);
        }


        public void GetPosition(out float x, out float y)
        {
            WorldLocation.GetPosition(out x, out y);
        }

        public void GetPosition(out float x, out float y, out float z)
        {
            WorldLocation.GetPosition(out x, out y, out z);
        }

        public void GetPosition(out float x, out float y, out float z, out float o)
        {
            WorldLocation.GetPosition(out x, out y, out z, out o);
        }

        public float GetExactDist2DSq(float x, float y)
        {
            return WorldLocation.GetExactDist2DSq(x, y);
        }

        public float GetExactDist2D(float x, float y)
        {
            return WorldLocation.GetExactDist2D(x, y);
        }

        public float GetExactDist2DSq(Position pos)
        {
            return WorldLocation.GetExactDist2DSq(pos);
        }

        public float GetExactDist2D(Position pos)
        {
            return WorldLocation.GetExactDist2D(pos);
        }

        public float GetExactDistSq(float x, float y, float z)
        {
            return WorldLocation.GetExactDistSq(x, y, z);
        }

        public float GetExactDist(float x, float y, float z)
        {
            return WorldLocation.GetExactDist(x, y, z);
        }

        public float GetExactDistSq(Position pos)
        {
            return WorldLocation.GetExactDistSq(pos);
        }

        public float GetExactDist(Position pos)
        {
            return WorldLocation.GetExactDist(pos);
        }


        public void GetPositionOffsetTo(Position endPos, Position retOffset)
        {
            WorldLocation.GetPositionOffsetTo(endPos, retOffset);
        }

        public Position GetPositionWithOffset(Position offset)
        {
            return WorldLocation.GetPositionWithOffset(offset);
        }


        public float GetAngle(Position pos)
        {
            return WorldLocation.GetAngle(pos);
        }

        public float GetAngle(float x, float y)
        {
            return WorldLocation.GetAngle(x, y);
        }

        public float GetRelativeAngle(Position pos)
        {
            return WorldLocation.GetRelativeAngle(pos);
        }

        public float GetRelativeAngle(float x, float y)
        {
            return WorldLocation.GetRelativeAngle(x, y);
        }

        public void GetSinCos(float x, float y, out float vsin, out float vcos)
        {
            WorldLocation.GetSinCos(x, y, out vsin, out vcos);
        }


        public bool IsInDist2D(float x, float y, float dist)
        {
            return WorldLocation.IsInDist2D(x, y, dist);
        }

        public bool IsInDist2D(Position pos, float dist)
        {
            return WorldLocation.IsInDist2D(pos, dist);
        }

        public bool IsInDist(float x, float y, float z, float dist)
        {
            return WorldLocation.IsInDist(x, y, z, dist);
        }

        public bool IsInDist(Position pos, float dist)
        {
            return WorldLocation.IsInDist(pos, dist);
        }

        public bool IsWithinBox(Position center, float xradius, float yradius, float zradius)
        {
            return WorldLocation.IsWithinBox(center, xradius, yradius, zradius);
        }

        public bool HasInArc(float arc, Position pos, float border = 2.0f)
        {
            return WorldLocation.HasInArc(arc, pos, border);
        }

        public bool HasInLine(Position pos, float width)
        {
            return WorldLocation.HasInLine(pos, width);
        }

        #endregion

        public virtual void DoUpdate(uint timeDiff) { }
        public override void RemoveFromWorld() { }
        public virtual void CleanupsBeforeDelete(bool finalCleanup = true) { }

        public void GetNearPoint2D(ref float x, ref float y, float distance, float absAngle) { }
        public void GetNearPoint(WorldEntity searcher, ref float x, ref float y, ref float z, float searcherSize, float distance2D, float absAngle) { }
        public void GetClosePoint(ref float x, ref float y, ref float z, float size, float distance2D = 0, float angle = 0) { }
        public void MovePosition(Position pos, float dist, float angle) { }
        public Position GetNearPosition(float dist, float angle) { return null; }
        public void MovePositionToFirstCollision(Position pos, float dist, float angle) { }
        public Position GetFirstCollisionPosition(float dist, float angle) { return null; }
        public Position GetRandomNearPosition(float radius) { return null; }
        public void GetContactPoint(WorldEntity obj, ref float x, ref float y, ref float z, float distance2D = EntityHelper.ContactDistance) { }
        public float GetObjectSize() { return 0.0f; }
        public void UpdateGroundPositionZ(float x, float y, ref float z) { }
        public void UpdateAllowedPositionZ(float x, float y, ref float z) { }
        public void GetRandomPoint(Position srcPos, float distance, ref float randX, ref float randY, ref float randZ) { }
        public Position GetRandomPoint(Position srcPos, float distance) { return null; }

        public string GetName() { return EntityName; }
        public void SetName(string newname) { EntityName = newname; }
    
        public float GetDistance(WorldEntity obj) { return 0.0f; }
        public float GetDistance(Position pos) { return 0.0f; }
        public float GetDistance(float x, float y, float z) { return 0.0f; }
        public float GetDistance2d(WorldEntity obj) { return 0.0f; }
        public float GetDistance2d(float x, float y) { return 0.0f; }
        public float GetDistanceZ(WorldEntity obj) { return 0.0f; }

        public bool IsSelfOrInSameMap(WorldEntity obj) { return false; }
        public bool IsWithinDist3d(float x, float y, float z, float dist) { return false; }
        public bool IsWithinDist3d(Position pos, float dist) { return false; }
        public bool IsWithinDist2d(float x, float y, float dist) { return false; }
        public bool IsWithinDist2d(Position pos, float dist) { return false; }
        // use only if you will sure about placing both object at same map
        public virtual bool IsWithinDist(WorldEntity obj, float dist2Compare, bool is3D = true) { return false; }
        public bool IsWithinDistInMap(WorldEntity obj, float dist2Compare, bool is3D = true) { return false; }
        public bool IsWithinLos(float x, float y, float z) { return false; }
        public bool IsWithinLosInMap(WorldEntity obj) { return false; }
        public bool GetDistanceOrder(WorldEntity obj1, WorldEntity obj2, bool is3D = true) { return false; }
        public bool IsInRange(WorldEntity obj, float minRange, float maxRange, bool is3D = true) { return false; }
        public bool IsInRange2D(float x, float y, float minRange, float maxRange) { return false; }
        public bool IsInRange3D(float x, float y, float z, float minRange, float maxRange) { return false; }
        public bool IsInFront(WorldEntity target, float arc = Mathf.PI) { return false; }
        public bool IsInBack(WorldEntity target, float arc = Mathf.PI) { return false; }
        public bool IsInBetween(WorldEntity obj1, WorldEntity obj2, float size = 0) { return false; }

        public virtual byte GetLevelForTarget(WorldEntity target) { return 1; }
        public float GetVisibilityRange() { return 0.0f; }
        public float GetSightRange(WorldEntity target = null) { return 0.0f; }
        public bool CanSeeOrDetect(WorldEntity obj, bool ignoreStealth = false, bool distanceCheck = false, bool checkAlert = false) { return false; }

        public virtual void SetMap(Map map)
        {
            Assert.IsNotNull(map);
            Assert.IsFalse(InWorld);

            if (currMap == map)
                return;

            currMap = map;
            if (IsWorldEntity)
                currMap.AddWorldObject(this);
        }

        public virtual void ResetMap() { throw new NotImplementedException(); }

        public TempSummon SummonCreature(uint id, Position pos, TempSummonType spwtype = TempSummonType.ManualDespawn, uint despwtime = 0, uint vehId = 0) { return null; }
        public TempSummon SummonCreature(uint id, float x, float y, float z, float ang = 0, TempSummonType spwtype = TempSummonType.ManualDespawn, uint despwtime = 0) { return null; }
        public GameObject SummonGameObject(uint entry, float x, float y, float z, float ang, float rotation0, float rotation1, float rotation2, float rotation3, uint respawnTime) { return null; }
        public Creature SummonTrigger(float x, float y, float z, float ang, uint dur, CreatureAI getAI = null) { return null; }
        public void SummonCreatureGroup(byte group, List<TempSummon> list = null) { }

        public Creature FindNearestCreature(uint entry, float range, bool alive = true) { return null; }
        public WorldEntity FindNearestGameObject(uint entry, float range) { return null; }
        public WorldEntity FindNearestGameObjectOfType(GameEntityTypes type, float range) { return null;}

        public void GetGameObjectListWithEntryInGrid(List<WorldEntity> list, uint uiEntry, float fMaxSearchRange) { }
        public void GetCreatureListWithEntryInGrid(List<Creature> list, uint uiEntry, float fMaxSearchRange) { }
        public void GetPlayerListInGrid(List<Player> list, float fMaxSearchRange) { }

        public void DestroyForNearbyPlayers() { }
        public virtual void UpdateObjectVisibility(bool forced = true) { }
        public virtual void UpdateObjectVisibilityOnCreate()
        {
            UpdateObjectVisibility();
        }

        //void BuildUpdate(UpdateDataMapType&) override;
        protected override void AddToEntityUpdate() { }
        protected override void RemoveFromEntityUpdate() { }

        public bool IsActiveObject() { return IsActive; }
        public void SetActive(bool isActiveObject) { }
        public void SetWorldObject(bool apply) { }
        public bool IsPermanentWorldObject() { return IsWorldEntity; }

        public void VisitNearbyObject(float radius, ref IEntityVisitor notifier) { if (InWorld) Map.VisitAll(X, Y, radius, ref notifier); }
        public void VisitNearbyGridObject(float radius, ref IGridVisitor notifier) { if (InWorld) Map.VisitGrid(X, Y, radius, ref notifier); }
        public void VisitNearbyWorldObject(float radius, ref IWorldVisitor notifier) { if (InWorld) Map.VisitWorld(X, Y, radius, ref notifier); }

        private bool CanNeverSee(WorldEntity obj) { return false; }
        private bool CanDetect(WorldEntity obj, bool ignoreStealth, bool checkAlert = false) { return false; }
        private bool CanDetectInvisibilityOf(WorldEntity obj) { return false; }
        private bool CanDetectStealthOf(WorldEntity obj, bool checkAlert = false) { return false; }
    }
}