using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Core;
using Client.Sound;
using Common;
using Core;
using Cysharp.Threading.Tasks;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Die Beam Clone",
        description: "Spawns clones near the player that rotate towards them and fire beam spells simultaneously.",
        story: "[Unit] spawns [CloneAmount] die beam clones",
        category: "Action/Unit",
        id: "b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3")]
    public class DieBeamCloneAction : BehaviourGraphActionWithUtility
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<List<GameObject>> CloneLocations;
        [SerializeReference] public BlackboardVariable<SoundEntry> WarningSound;
        [SerializeReference] public BlackboardVariable<SpellInfo> BeamSpellInfo;
        [SerializeReference] public BlackboardVariable<CreatureInfo> CreatureInfo;
        [SerializeReference] public BlackboardVariable<WorldEntityPrefab> Prefab;
        [SerializeReference] public BlackboardVariable<string> CustomCloneName;
        [SerializeReference] public BlackboardVariable<float> CustomCloneScale = new(1.0f);
        [SerializeReference] public BlackboardVariable<float> SpawnInCircleRadiusMin = new(2.0f);
        [SerializeReference] public BlackboardVariable<float> SpawnInCircleRadius = new(10.0f);
        [SerializeReference] public BlackboardVariable<float> LaunchHeight = new(2f);
        [SerializeReference] public BlackboardVariable<float> TargetHeight = new(1f);
        [SerializeReference] public BlackboardVariable<float> WaitAfterWarning = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> WaitAfterCasting = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> RotationSpeed = new(180f);
        [SerializeReference] public BlackboardVariable<int> CloneAmount = new(1);
        [SerializeReference] public BlackboardVariable<SpellCastFlags> CastFlags;

        private bool isRunning;
        private CancellationTokenSource cancellationSource;
        private readonly List<Creature> spawnedClones = new();

        protected override Status OnStart()
        {
            cancellationSource = new CancellationTokenSource();
            RunAsync(cancellationSource.Token).Forget();

            async UniTask RunAsync(CancellationToken token)
            {
                isRunning = true;
                try
                {
                    await UpdateAsync(token);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    isRunning = false;
                }
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return isRunning ? Status.Running : Status.Success;
        }

        protected override void OnEnd()
        {
            isRunning = false;
            cancellationSource?.Cancel();
            cancellationSource?.Dispose();
            cancellationSource = null;

            spawnedClones.ForEach(clone => World.UnitManager.Destroy(clone));
            spawnedClones.Clear();

            base.OnEnd();
        }

        private async UniTask UpdateAsync(CancellationToken token)
        {
            if (CloneLocations.Value.Count < 2)
                return;

            Player player = World.UnitManager.FindNearby<Player>(Unit.Value.Position, p => !p.IsDead);
            if (player == null || !player.IsValid)
                return;

            WarningSound.Value.Play(player.Position);

            List<GameObject> potentialLocations = new List<GameObject>(CloneLocations.Value);
            List<GameObject> prefferedLocations = new List<GameObject>(CloneLocations.Value);
            float sqrRadiusMax = SpawnInCircleRadius.Value * SpawnInCircleRadius.Value;
            float sqrRadiusMin = SpawnInCircleRadiusMin.Value * SpawnInCircleRadiusMin.Value;
            prefferedLocations.RemoveAll(item =>
            {
                float sqrDistance = Vector3.SqrMagnitude(player.Position - item.transform.position);
                return sqrDistance > sqrRadiusMax || sqrDistance < sqrRadiusMin;
            });

            int clonesLeftToSpawn = CloneAmount.Value;
            while (potentialLocations.Count > 0 && clonesLeftToSpawn > 0)
            {
                Vector3 spawnTarget = SelectSpawnTarget(player, potentialLocations, prefferedLocations, out GameObject selectedLocation);
                Vector3 lookDirection = player.Position - spawnTarget;
                lookDirection.y = 0;

                var spawnedClone = World.UnitManager.Create<Creature>(Prefab.Value, new Creature.CreateToken
                {
                    Map = Unit.Value.Map,
                    Position = spawnTarget,
                    Rotation = lookDirection.sqrMagnitude > Mathf.Epsilon
                        ? Quaternion.LookRotation(lookDirection)
                        : Unit.Value.Rotation,
                    OriginalAIInfoId = 0,
                    DeathState = DeathState.Alive,
                    FreeForAll = false,
                    ClassType = ClassType.Warrior,
                    ModelId = CreatureInfo.Value.ModelId,
                    OriginalModelId = CreatureInfo.Value.ModelId,
                    FactionId = Unit.Value.Faction.FactionId,
                    CreatureInfoId = CreatureInfo.Value.Id,
                    CustomName = CustomCloneName.Value,
                    Scale = CustomCloneScale.Value
                });

                potentialLocations.Remove(selectedLocation);
                prefferedLocations.Remove(selectedLocation);
                spawnedClones.Add(spawnedClone);
                clonesLeftToSpawn--;
            }

            float waitRemaining = WaitAfterWarning.Value;
            while (waitRemaining > 0)
            {
                foreach (Creature clone in spawnedClones)
                {
                    Vector3 direction = player.Position - clone.Position;
                    direction.y = 0;
                    if (direction.sqrMagnitude > Mathf.Epsilon)
                        clone.CharacterController.Motor.RotateCharacter(Quaternion.RotateTowards(
                            clone.Rotation,
                            Quaternion.LookRotation(direction), RotationSpeed.Value * Time.deltaTime));
                }

                waitRemaining -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }

            foreach (Creature clone in spawnedClones)
            {
                Vector3 source = clone.Position + Vector3.up * LaunchHeight.Value;
                Vector3 target = player.Position + Vector3.up * TargetHeight.Value;
                Vector3 direction = target - source;

                clone.Spells.CastSpell(
                    BeamSpellInfo.Value,
                    new SpellCastingOptions(
                        new SpellExplicitTargets { Source = source },
                        targetingSource: source,
                        targetingRotation: Quaternion.LookRotation(direction.sqrMagnitude > Mathf.Epsilon ? direction : clone.transform.forward),
                        castFlags: CastFlags.Value));
            }

            await UniTask.WaitForSeconds(WaitAfterCasting.Value, cancellationToken: token);
        }

        private Vector3 SelectSpawnTarget(
            Player target,
            List<GameObject> potentialLocations,
            List<GameObject> prefferedLocations,
            out GameObject selectedLocation)
        {
            if (prefferedLocations.Count > 0)
            {
                selectedLocation = RandomUtils.GetRandomElement(prefferedLocations);
                return SampleLocation(selectedLocation.transform.position);
            }

            List<(GameObject, float)> locations = new();
            foreach (GameObject cloneLocation in potentialLocations)
                locations.Add((cloneLocation, Vector3.Distance(target.Position, cloneLocation.transform.position)));

            locations.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            selectedLocation = locations.Count > 1 ? locations[1].Item1 : locations[0].Item1;
            return SampleLocation(selectedLocation.transform.position);

            Vector3 SampleLocation(Vector3 locationToSample)
            {
                if (NavMesh.SamplePosition(locationToSample, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                    locationToSample = hit.position;

                return locationToSample;
            }
        }
    }
}
