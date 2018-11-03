using System;
using System.Collections.Generic;

namespace Core
{
    public class GameEntity : WorldEntity, IGridEntity<GameEntity>
    {
        public GridReference<GameEntity> GridRef { get; private set; }

        public GameEntityModel Model { get; set; }

        public uint SpellId { get; protected set; }
        public long RespawnTime { get; protected set; }                         // (secs) time of next respawn (or despawn if GO have owner()),
        public uint RespawnDelayTime { get; protected set; }                    // (secs) if 0 then current GO state no dependent from timer
        public bool SpawnedByDefault { get; protected set; }
        public long CooldownTime { get; protected set; }                        // used as internal reaction delay time store (not state change reaction).
        // For traps this: spell casting cooldown, for doors/buttons: reset time.
        public List<Guid> SkillupList { get; protected set; }

        public List<Guid> UniqueUsers { get; protected set; }
        public int Usetimes { get; protected set; }

        public Guid SpawnId { get; protected set; }
        public GameEntityTemplate GoInfo { get; protected set; }
        public GameEntityData GoData { get; protected set; }

        public ulong Rotation { get; protected set; }
        public Position StationaryPosition { get; protected set; }

        public GameEntityAI AI { get; private set; }
        public short AnimKitId { get; private set; }


        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<GameEntity> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        //void BuildValuesUpdate(ObjectUpdateType updatetype, ByteBuffer* data, Player target) const override;

        public override void AddToWorld() { }
        public override void RemoveFromWorld() { }
        public override void CleanupsBeforeDelete(bool finalCleanup = true) { }

        public bool Create(uint nameId, Map map, uint phaseMask, float x, float y, float z, float ang, float rotation0,
            float rotation1, float rotation2, float rotation3, uint animprogress, GoState goState, uint artKit = 0) { return false; }
        public override void DoUpdate(uint pTime) { }

        public GameEntityTemplate GetGoInfo() { return GoInfo; }
        public GameEntityData GetGoData() { return GoData; }

        public bool IsTransport() { return false; }
        public bool IsDynTransport() { return false; }
        public bool IsDestructibleBuilding() { return false; }

        public Guid GetSpawnId() { return SpawnId; }

        public void UpdateRotationFields(float rotation2 = 0.0f, float rotation3 = 0.0f) { }

        public void SaveToDb() { }
        public void SaveToDb(uint mapid, uint spawnMask, uint phaseMask) { }
        public bool LoadFromDb(Guid spawnId, Map map) { return LoadGameObjectFromDb(spawnId, map, false); }
        public bool LoadGameObjectFromDb(Guid spawnId, Map map, bool addToMap = true) { return false; }
        public void DeleteFromDb() { }

        public void SetOwnerGUID(Guid owner)
        {
            if (owner != Guid.Empty && GetOwnerGUID() != Guid.Empty && GetOwnerGUID() != owner)
            {
                throw new ArgumentException("Game entity owner already found and different than expected owner - remove object from old owner!", "owner");
            }
            SpawnedByDefault = false; // all object with owner is despawned after delay
            SetGuidValue(EntityFields.GameEntityCreatedBy, owner);
        }
        public Guid GetOwnerGUID() { return GetGuidValue(EntityFields.GameEntityCreatedBy); }
        public Unit GetOwner() { return null; }

        public void SetSpellId(uint id)
        {
            SpawnedByDefault = false;                     // all summoned object is despawned after delay
            SpellId = id;
        }
        public uint GetSpellId() { return SpellId;}

        public long GetRespawnTime() { return RespawnTime; }
        public long GetRespawnTimeEx() 
        {
            long now = TimeHelper.NowInMilliseconds;
            if (RespawnTime > now)
                return RespawnTime;
            else
                return now;
        }

        public void SetRespawnTime(int respawn)
        {
            RespawnTime = respawn > 0 ? TimeHelper.NowInMilliseconds + respawn : 0;
            RespawnDelayTime = respawn > 0 ? (uint)respawn : 0;
        }
        public void Respawn() { }
        public bool IsSpawned()
        {
            return RespawnDelayTime == 0 ||
                   (RespawnTime > 0 && !SpawnedByDefault) ||
                   (RespawnTime == 0 && SpawnedByDefault);
        }
        public bool IsSpawnedByDefault() { return SpawnedByDefault; }
        public void SetSpawnedByDefault(bool b) { SpawnedByDefault = b; }
        public uint GetRespawnDelay() { return RespawnDelayTime; }
        public void Refresh() { }
        public void Delete() { }
        public void SendGameObjectDespawn() { }

        public GameEntityTypes GetGoType() { return (GameEntityTypes)GetByteValue(EntityFields.GameEntityInfo, 1); }
        public void SetGoType(GameEntityTypes type) { SetByteValue(EntityFields.GameEntityInfo, 1, (byte)type); }
        public GoState GetGoState() { return (GoState)GetByteValue(EntityFields.GameEntityInfo, 0); }

        public void SetGoState(GoState state) { }
        public virtual uint GetTransportPeriod() { return 0; }
        public void SetTransportState(GoState state, uint stopFrame = 0) { }
        public byte GetGoArtKit() { return GetByteValue(EntityFields.GameEntityInfo, 2); }
        public void SetGoArtKit(byte artkit) { }
        public byte GetGoAnimProgress() { return GetByteValue(EntityFields.GameEntityInfo, 3); }
        public void SetGoAnimProgress(byte animprogress) { SetByteValue(EntityFields.GameEntityInfo, 3, animprogress); }
        public static void SetGoArtKit(byte artkit, GameEntity go, Guid lowguid = default(Guid)) { }

        public void CastSpell(Unit target, uint spell, bool triggered = true) { }
        public void SendCustomAnim(uint anim) { }
        public bool IsInRange(float x, float y, float z, float radius) { return false; }

        public void ModifyHealth(int change, Unit attackerOrHealer = null, uint spellId = 0) { }
        public void SetDestructibleState(GameEntityDestructibleState state, Player eventInvoker = null, bool setHealth = false) { }
        public GameEntityDestructibleState GetDestructibleState()
        {
            if (HasFlag(EntityFields.GameEntityInfo, (uint)GameEntityDestructibleState.Destroyed))
                return GameEntityDestructibleState.Destroyed;
            return HasFlag(EntityFields.GameEntityInfo, (uint)GameEntityDestructibleState.Damaged) ?
                GameEntityDestructibleState.Damaged : GameEntityDestructibleState.Intact;
        }

        public void EventInform(uint eventId, WorldEntity invoker = null) { }

        public ulong GetRotation() { return Rotation; }
        public virtual uint GetScriptId() { return GetGoInfo().ScriptId; }
        public GameEntityAI GetAI() { return AI; }

        public string GetAIName() { return null; }
        public void SetDisplayId(uint displayid) { }
        public uint GetDisplayId() { return GetUintValue(EntityFields.GameEntityDisplayId); }
    }
}