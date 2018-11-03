namespace Core
{
    public abstract class GameEntityTemplate
    {
        public abstract GameEntityTypes Type { get; }

        public uint Entry { get; set; }
        public uint ScriptId { get; set; }
        public int RequiredLevel { get; set; }

        public uint DisplayId { get; set; }
        public float Size { get; set; }
        public string Name { get; set; }
        public string AIName { get; set; }
        public string IconName { get; set; }
        public string CastBarCaption { get; set; }


        public virtual bool IsDespawnAtAction => false;
        public virtual bool IsUsableMounted => false;
        public virtual uint LockId => 0;
        public virtual bool DespawnPossibility => true;
        public virtual uint UseCharges => 0;
        public virtual uint LinkedEntityEntry => 0;
        public virtual uint AutoCloseTime => 0;
        public virtual uint LootId => 0;
        public virtual uint GossipMenuId => 0;
        public virtual uint EventScriptId => 0;
        public virtual uint CastCooldown => 0;
    }
}