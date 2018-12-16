using System.Collections.Generic;

namespace Core
{
    public static class EntityAccessor
    {
        private static readonly object PlayerLock = new object();
        private static readonly Dictionary<ulong, Player> Players = new Dictionary<ulong, Player>(10);

        public static WorldEntity FindWorldEntityOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<WorldEntity>(networkId);
        }

        public static Corpse FindCorpseOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<Corpse>(networkId);
        }

        public static GameEntity FindGameEntityOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<GameEntity>(networkId);
        }

        public static DynamicEntity FindDynamicEntityOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<DynamicEntity>(networkId);
        }

        public static AreaTrigger FindAreaTriggerOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<AreaTrigger>(networkId);
        }

        public static Unit FindUnitOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<Unit>(networkId) ?? FindPlayerOnSameMap(worldEntity.Map, networkId);
        }

        public static Creature FindCreatureOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<Creature>(networkId);
        }

        public static Pet FindPetOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return worldEntity.Map.FindMapEntity<Pet>(networkId);
        }

        public static Player FindPlayerOnSameMap(WorldEntity worldEntity, ulong networkId)
        {
            return FindPlayerOnSameMap(worldEntity.Map, networkId);
        }

        public static Player FindPlayerOnSameMap(Map map, ulong networkId)
        {
            Player player = Players.LookupEntry(networkId);
            return player != null && player.Map == map ? player : null;
        }

        public static Player FindConnectedPlayer(ulong networkId)
        {
            lock (PlayerLock)
                return Players.LookupEntry(networkId);
        }

        public static Player FindConnectedPlayerByName(string name)
        {
            lock (PlayerLock)
            {
                string lowerName = name.ToLower();
                foreach (var playerEntry in Players)
                    if (playerEntry.Value.Name.ToLower() == lowerName)
                        return playerEntry.Value;

                return null;
            }
        }


        public static void AddEntity(Player player)
        {
            lock (PlayerLock)
                Players.Add(player.NetworkId, player);
        }

        public static void RemoveEntity(Player player)
        {
            lock (PlayerLock)
                Players.Remove(player.NetworkId);
        }
    }
}