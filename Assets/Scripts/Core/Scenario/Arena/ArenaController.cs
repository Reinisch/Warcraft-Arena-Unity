using System.Collections.Generic;
using Assets.Scripts.Core;
using Common;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Server-authoritative controller for a generic arena fight (WoW-style).
    /// The scenario graph runs server-only.
    /// </summary>
    public sealed class ArenaController : MonoBehaviour
    {
        private sealed class Tally
        {
            public int Damage;
            public int Healing;
        }

        private World world;
        private Map map;
        private EventBus eventBus;

        private ScenarioDefinition scenario;
        private FactionDefinition factionA;
        private FactionDefinition factionB;
        private IReadOnlyList<Transform> spawnPointsA;
        private IReadOnlyList<Transform> spawnPointsB;
        private int playersPerTeam;
        private float warmupSeconds;

        private SpellInfo preparationSpell; // Optional arena spell applied to each player during warmup.
        private UnitAttributeDefinition arenaAttributes; // Optional attributes to each participant on assignment.
        private WorldEntityPrefab playerAiPrefab; // Optional Player AI used to balance the teams on start.
        private UnitInfoAI playerAiInfo;

        private readonly List<Player> teamA = new();
        private readonly List<Player> teamB = new();
        private readonly Dictionary<Player, Tally> tallies = new();

        private float warmupRemaining;
        private int lastBroadcastSecond = int.MinValue;
        private bool initialized;
        private bool? forcedTeamA; // when spawning a bot for a side, forces AssignPlayer onto that side instead

        public ArenaPhase Phase { get; private set; } = ArenaPhase.Warmup;
        public ArenaMatchResult Result { get; private set; } = ArenaMatchResult.Undecided;
        public bool BothTeamsReady => teamA.Count >= playersPerTeam && teamB.Count >= playersPerTeam;

        public ArenaState CurrentState => new ArenaState(
            Phase, Mathf.Max(0f, warmupRemaining),
            AliveCount(teamA), teamA.Count,
            AliveCount(teamB), teamB.Count);

        public void Initialize(World world, Map map, EventBus eventBus,
            FactionDefinition factionA, FactionDefinition factionB,
            IReadOnlyList<Transform> spawnPointsA, IReadOnlyList<Transform> spawnPointsB,
            ScenarioDefinition scenario, float warmupSeconds, SpellInfo preparationSpell,
            WorldEntityPrefab playerAiPrefab, UnitInfoAI playerAiInfo,
            UnitAttributeDefinition arenaAttributes)
        {
            this.world = world;
            this.map = map;
            this.eventBus = eventBus;
            this.factionA = factionA;
            this.factionB = factionB;
            this.spawnPointsA = spawnPointsA;
            this.spawnPointsB = spawnPointsB;
            this.warmupSeconds = Mathf.Max(0f, warmupSeconds);
            this.preparationSpell = preparationSpell;
            this.playerAiPrefab = playerAiPrefab;
            this.playerAiInfo = playerAiInfo;
            this.arenaAttributes = arenaAttributes;
            this.scenario = scenario;

            playersPerTeam = Mathf.Max(1, scenario.TeamSize);
            warmupRemaining = this.warmupSeconds;
            world.UnitManager.EventEntityAttached += OnUnitAttached;
            world.UnitManager.EventEntityDetach += OnUnitDetached;

            eventBus.RegisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnDamageDone);
            eventBus.RegisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnHealingDone);

            foreach (Unit unit in world.UnitManager.Entities)
                if (unit is Player player)
                    AssignPlayer(player);

            initialized = true;
            RaiseStateChanged();
        }

        private void OnDestroy()
        {
            if (world == null)
                return;

            world.UnitManager.EventEntityAttached -= OnUnitAttached;
            world.UnitManager.EventEntityDetach -= OnUnitDetached;

            if (eventBus != null)
            {
                eventBus.UnregisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnDamageDone);
                eventBus.UnregisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnHealingDone);
            }
        }

        public bool IsMatchOver()
        {
            if (Phase == ArenaPhase.Ended)
                return true;
            if (Phase != ArenaPhase.InProgress)
                return false;

            return AliveCount(teamA) == 0 || AliveCount(teamB) == 0;
        }

        public void TriggerSpellOnAllPlayers(SpellInfo spell)
        {
            if (spell == null)
                return;

            foreach (Player player in AllPlayers())
                if (player != null)
                    player.Spells.TriggerSpell(spell, player);
        }

        public void TriggerSpellOnPlayer(Player player, SpellInfo spell)
        {
            if (player != null && spell != null)
                player.Spells.TriggerSpell(spell, player);
        }

        public ArenaMatchResult EndMatch()
        {
            if (Phase == ArenaPhase.Ended)
                return Result;

            int aliveA = AliveCount(teamA);
            int aliveB = AliveCount(teamB);

            if (aliveA > 0 && aliveB == 0)
                Result = ArenaMatchResult.TeamA;
            else if (aliveB > 0 && aliveA == 0)
                Result = ArenaMatchResult.TeamB;
            else
                Result = ArenaMatchResult.Draw; // mutual elimination or a forced end with both alive

            Phase = ArenaPhase.Ended;

            RaiseStateChanged();
            eventBus.ExecuteEvent(GameEvents.ServerArenaMatchEnded, BuildReport());
            return Result;
        }

        private ArenaMatchReport BuildReport()
        {
            var participants = new List<ArenaParticipantStats>(teamA.Count + teamB.Count);

            foreach (Player player in teamA)
                if (player != null)
                    participants.Add(ToStats(player, true));
            foreach (Player player in teamB)
                if (player != null)
                    participants.Add(ToStats(player, false));

            return new ArenaMatchReport(Result, participants);
        }

        private ArenaParticipantStats ToStats(Player player, bool isTeamA)
        {
            int damage = 0, healing = 0;
            if (tallies.TryGetValue(player, out Tally tally))
            {
                damage = tally.Damage;
                healing = tally.Healing;
            }

            return new ArenaParticipantStats(player, isTeamA, damage, healing);
        }

        private void Update()
        {
            if (initialized && Phase == ArenaPhase.Warmup)
                TickWarmup(Time.deltaTime);
        }

        private void TickWarmup(float deltaTime)
        {
            warmupRemaining -= deltaTime;
            if (BothTeamsReady && warmupRemaining > scenario.MinArenaWaitTime)
                warmupRemaining = scenario.MinArenaWaitTime;

            if (warmupRemaining <= 0f)
            {
                BeginMatch();
                return;
            }

            int second = Mathf.CeilToInt(Mathf.Max(0f, warmupRemaining));
            if (second != lastBroadcastSecond)
            {
                lastBroadcastSecond = second;
                RaiseStateChanged();
            }
        }

        private void OnDamageDone(Unit caster, Unit target, int amount, HitType hitType, Vector3? hitPosition)
        {
            if (caster is Player player && IsParticipant(player))
                GetTally(player).Damage += amount;
        }

        private void OnHealingDone(Unit healer, Unit target, int amount, bool isCrit)
        {
            if (healer is Player player && IsParticipant(player))
                GetTally(player).Healing += amount;
        }

        private bool IsParticipant(Player player) => teamA.Contains(player) || teamB.Contains(player);

        private Tally GetTally(Player player)
        {
            if (!tallies.TryGetValue(player, out Tally tally))
                tallies[player] = tally = new Tally();

            return tally;
        }

        private void OnUnitAttached(Unit unit)
        {
            if (unit is Player player)
                AssignPlayer(player);
        }

        private void OnUnitDetached(Unit unit)
        {
            if (unit is Player player && (teamA.Remove(player) || teamB.Remove(player)))
                RaiseStateChanged(); // a team emptying mid-match is picked up by IsMatchOver next tick
        }

        private void AssignPlayer(Player player)
        {
            if (teamA.Contains(player) || teamB.Contains(player) || player.Map != map)
                return;

            // Forced side for a team-fill bot; otherwise balance onto the smaller team.
            bool toTeamA = forcedTeamA ?? teamA.Count <= teamB.Count;
            forcedTeamA = null;

            List<Player> roster = toTeamA ? teamA : teamB;
            FactionDefinition faction = toTeamA ? factionA : factionB;
            IReadOnlyList<Transform> spawns = toTeamA ? spawnPointsA : spawnPointsB;

            roster.Add(player);

            player.FreeForAll = false;
            if (faction != null)
                player.Faction = faction;

            player.ApplyAttributeDefinition(arenaAttributes);

            Transform spawn = ResolveSpawn(spawns, roster.Count - 1);
            if (spawn != null)
            {
                player.SetFacing(spawn.rotation);
                player.Teleport(spawn.position);
            }

            // Late joiners (match already underway) spawn straight into the fight, not frozen.
            if (Phase == ArenaPhase.Warmup)
                ApplyPreparation(player, true);

            RaiseStateChanged();
        }

        private void BeginMatch()
        {
            if (Phase != ArenaPhase.Warmup)
                return;

            Phase = ArenaPhase.InProgress;
            warmupRemaining = 0f;

            foreach (Player player in AllPlayers())
                if (player != null)
                    ApplyPreparation(player, false);

            FillTeamsWithAi();
            RaiseStateChanged();
        }

        private void FillTeamsWithAi()
        {
            if (playerAiPrefab == null || playerAiInfo == null)
                return;

            int needA = Mathf.Max(0, playersPerTeam - teamA.Count);
            for (int i = 0; i < needA; i++)
                SpawnAiPlayer(toTeamA: true);

            int needB = Mathf.Max(0, playersPerTeam - teamB.Count);
            for (int i = 0; i < needB; i++)
                SpawnAiPlayer(toTeamA: false);
        }

        private void SpawnAiPlayer(bool toTeamA)
        {
            // Force this bot onto the chosen side; AssignPlayer (fired synchronously on attach) reads + clears it.
            forcedTeamA = toTeamA;
            try
            {
                CreateAiPlayer();
            }
            finally
            {
                forcedTeamA = null;
            }
        }

        private void CreateAiPlayer()
        {
            int factionId = 0;
            if (factionA != null)
                factionId = factionA.FactionId;
            else if (factionB != null)
                factionId = factionB.FactionId;

            world.UnitManager.Create<Player>(playerAiPrefab, new Player.CreateToken
            {
                Map = map,
                Position = map.Settings.transform.position,
                Rotation = Quaternion.identity,
                OriginalAIInfoId = playerAiInfo.Id,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = ClassType.Mage,
                ModelId = 1,
                OriginalModelId = 1,
                FactionId = factionId,
                PlayerName = "Arena Bot",
                Scale = 1,
            });
        }

        private void ApplyPreparation(Player player, bool applied)
        {
            if (preparationSpell != null)
            {
                if (applied)
                    player.Spells.TriggerSpell(preparationSpell, player);
                else
                    player.Auras.RemoveAuraWithSpellInfo(preparationSpell, AuraRemoveMode.Default);
            }
        }

        private void RaiseStateChanged() => eventBus.ExecuteEvent(GameEvents.ServerArenaStateChanged, CurrentState);

        private IEnumerable<Player> AllPlayers()
        {
            foreach (Player player in teamA)
                yield return player;
            foreach (Player player in teamB)
                yield return player;
        }

        private static int AliveCount(List<Player> roster)
        {
            int count = 0;
            for (int i = 0; i < roster.Count; i++)
                if (roster[i] != null && roster[i].IsAlive)
                    count++;

            return count;
        }

        private static Transform ResolveSpawn(IReadOnlyList<Transform> spawns, int slot)
        {
            if (spawns == null || spawns.Count == 0)
                return null;

            return spawns[slot % spawns.Count];
        }
    }
}
