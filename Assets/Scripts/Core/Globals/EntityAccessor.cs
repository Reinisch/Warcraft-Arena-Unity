using System;
using System.Collections.Generic;

namespace Core
{
    public static class EntityAccessor
    {
        private static readonly object PlayerLock = new object();
        private static readonly Dictionary<Guid, Player> Players = new Dictionary<Guid, Player>(1000);

        public static WorldEntity FindWorldEntityOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<WorldEntity>(findEntityGuid);
        }

        public static Corpse FindCorpseOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<Corpse>(findEntityGuid);
        }

        public static GameEntity FindGameEntityOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<GameEntity>(findEntityGuid);
        }

        public static DynamicEntity FindDynamicEntityOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<DynamicEntity>(findEntityGuid);
        }

        public static AreaTrigger FindAreaTriggerOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<AreaTrigger>(findEntityGuid);
        }

        public static Unit FindUnitOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<Unit>(findEntityGuid) ?? FindPlayerOnSameMap(worldEntity.Map, findEntityGuid);
        }

        public static Creature FindCreatureOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<Creature>(findEntityGuid);
        }

        public static Pet FindPetOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return worldEntity.Map.FindMapEntity<Pet>(findEntityGuid);
        }

        public static Player FindPlayerOnSameMap(WorldEntity worldEntity, Guid findEntityGuid)
        {
            return FindPlayerOnSameMap(worldEntity.Map, findEntityGuid);
        }

        public static Player FindPlayerOnSameMap(Map map, Guid findEntityGuid)
        {
            Player player = Players.LookupEntry(findEntityGuid);
            return player != null && player.InWorld && player.Map == map ? player : null;
        }

        public static Player FindPlayerInWorld(Guid guid)
        {
            lock (PlayerLock)
            {
                Player player = Players.LookupEntry(guid);
                return player != null && player.InWorld ? player : null;
            }
        }

        public static Player FindPlayerInWorldByName(string name)
        {
            lock (PlayerLock)
            {
                string lowerName = name.ToLower();
                foreach (var playerEntry in Players)
                    if (playerEntry.Value.InWorld && playerEntry.Value.GetName().ToLower() == lowerName)
                        return playerEntry.Value;

                return null;
            }
        }


        public static Player FindConnectedPlayer(Guid guid)
        {
            lock (PlayerLock)
                return Players.LookupEntry(guid);
        }

        public static Player FindConnectedPlayerByName(string name)
        {
            lock (PlayerLock)
            {
                string lowerName = name.ToLower();
                foreach (var playerEntry in Players)
                    if (playerEntry.Value.GetName().ToLower() == lowerName)
                        return playerEntry.Value;

                return null;
            }
        }


        public static void AddEntity(Player player)
        {
            lock (PlayerLock)
                Players.Add(player.Guid, player);
        }

        public static void RemoveEntity(Player player)
        {
            lock (PlayerLock)
                Players.Remove(player.Guid);
        }
    }
}