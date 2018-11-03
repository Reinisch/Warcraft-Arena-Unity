namespace Core
{
    public class GameEntityAI
    {
        protected GameEntity GameEntity { get; set; }

        public GameEntityAI(GameEntity entity)
        {
            GameEntity = entity;
        }


        public virtual void UpdateAI(uint diff)
        {
        }

        public virtual void InitializeAI()
        {
            Reset();
        }

        public virtual void Reset()
        {
        }


        // Pass parameters between AI
        public virtual void DoAction(int param = 0)
        {
        }

        public virtual void SetGUID(ulong guid, int id = 0)
        {
        }

        public virtual ulong GetGUID(int id = 0)
        {
            return 0;
        }


        public static int Permissible(GameEntity gameEntity)
        {
            return 0;
        }


        public virtual bool GossipHello(Player player, bool isUse)
        {
            return false;
        }

        public virtual bool GossipSelect(Player player, uint sender, uint action)
        {
            return false;
        }

        public virtual bool GossipSelectCode(Player player, uint sender, uint action, char code)
        {
            return false;
        }

        public virtual void Destroyed(Player player, uint eventId)
        {
        }

        public virtual uint GetData(uint id)
        {
            return 0;
        }

        public virtual void SetData64(uint id, ulong value)
        {
        }

        public virtual ulong GetData64(uint id)
        {
            return 0;
        }

        public virtual void SetData(uint id, uint value)
        {
        }

        public virtual void OnGameEvent(bool start, ushort eventId)
        {
        }

        public virtual void OnStateChanged(uint state, Unit unit)
        {
        }

        public virtual void EventInform(uint eventId)
        {
        }
    }
}