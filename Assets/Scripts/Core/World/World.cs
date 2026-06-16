using Assets.Scripts.Core;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Core
{
    public class World
    {
        [Inject]
        internal SpellManager SpellManager { get; private set; }

        [Inject]
        internal ProjectileLauncher ProjectileLauncher { get; private set; }

        [Inject]
        public UnitManager UnitManager { get; private set; }

        [Inject]
        public PlayerManager PlayerManager { get; private set; }

        [Inject]
        public MapController MapController { get; private set; }

        public MovementMode DefaultMovementMode { get; set; } = MovementMode.Rpg;

        // Class the LOCAL (host/single-player) player spawns as — chosen in the lobby. Remote clients send their
        public ClassType LocalPlayerClass { get; set; } = ClassType.Mage;

        public bool HasServerLogic { get; private set; } = true;
        public bool HasClientLogic { get; private set; } = true;

        public void ConfigureLogic(bool hasServerLogic, bool hasClientLogic)
        {
            HasServerLogic = hasServerLogic;
            HasClientLogic = hasClientLogic;
        }

        /// <summary>
        /// Recreates a unit from a replicated <see cref="UnitSnapshot"/> (client-side).
        /// </summary>
        public Unit SpawnFromState(UnitSnapshot snapshot, WorldEntityPrefab prefab, Map map, bool asLocalPlayer = false)
        {
            Entity.CreateToken token = snapshot.ToCreateToken(map);

            if (snapshot.Kind == UnitSnapshotKind.Player)
            {
                // The owned player becomes the local PlayerManager.Player; other players are plain units.
                return asLocalPlayer
                    ? PlayerManager.Create(token)
                    : UnitManager.Create<Player>(prefab, token);
            }

            return UnitManager.Create<Creature>(prefab, token);
        }

        /// <summary>
        /// Server-side: spawn an authoritative Player for a freshly connected client.
        /// </summary>
        public Player SpawnConnectionPlayer(WorldEntityPrefab prefab, Map map, ClassType classType)
        {
            Player host = PlayerManager.Player;
            UnityEngine.Vector3 position = host != null ? host.Position + new UnityEngine.Vector3(2f, 0f, 0f) : UnityEngine.Vector3.zero;
            int baseModelId = host != null ? host.OriginalModelId : 0;

            Player player = UnitManager.Create<Player>(prefab, new Player.CreateToken
            {
                Map = map,
                Position = position,
                Rotation = UnityEngine.Quaternion.identity,
                OriginalAIInfoId = 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = classType,
                ModelId = baseModelId,
                OriginalModelId = baseModelId,
                FactionId = host != null ? host.Faction.FactionId : 0,
                PlayerName = "Player",
                Scale = 1,
            });
            player.MovementMode = DefaultMovementMode;
            return player;
        }

        public virtual void Dispose(bool applicationQuitting = false)
        {
            if (!applicationQuitting)
                MapController.UnloadAllAsync().Forget();

            MapController.Dispose();
            ProjectileLauncher.Dispose();
            SpellManager.Dispose();
            UnitManager.Dispose();
        }

        public virtual void DoUpdate(int deltaTime)
        {
            foreach (Map map in MapController.LoadedMaps)
                map.DoUpdate(deltaTime);

            UnitManager.DoUpdate(deltaTime);
            SpellManager.DoUpdate(deltaTime);
            ProjectileLauncher.DoUpdate(deltaTime);
        }
    }
}