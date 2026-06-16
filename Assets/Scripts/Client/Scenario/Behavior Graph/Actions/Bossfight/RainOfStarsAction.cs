using System;
using System.Collections.Generic;
using System.Threading;
using Client.Sound;
using Common;
using Core;
using Cysharp.Threading.Tasks;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Rain Of Stars",
        description: "Launches a pattern of meteor spells around the nearest alive player.",
        story: "[Unit] performs rain of stars with [MeteorSpellInfo]",
        category: "Action/Unit",
        id: "a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2")]
    public class RainOfStarsAction : BehaviourGraphActionWithUtility
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<SoundEntry> WarningSound;
        [SerializeReference] public BlackboardVariable<SpellInfo> MeteorSpellInfo;
        [SerializeReference] public BlackboardVariable<float> WaitAfterWarning = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> WaitAfterCasting = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> MajorCircleRadius = new(10f);
        [SerializeReference] public BlackboardVariable<float> MinorCircleRadius = new(9f);
        [SerializeReference] public BlackboardVariable<int> MinorCircles = new(10);
        [SerializeReference] public BlackboardVariable<int> ProjectilesPerMinor = new(5);
        [SerializeReference] public BlackboardVariable<float> MeteorRandomSourceRadius = new(10f);
        [SerializeReference] public BlackboardVariable<float> MeteorHeightFar = new(30f);
        [SerializeReference] public BlackboardVariable<float> MeteorHeightNear = new(30f);
        [SerializeReference] public BlackboardVariable<float> SpellCastDelayMin = new(0.01f);
        [SerializeReference] public BlackboardVariable<float> SpellCastDelayMax = new(0.5f);
        [SerializeReference] public BlackboardVariable<SpellCastFlags> CastFlags;

        private bool isRunning;
        private CancellationTokenSource cancellationSource;

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

            base.OnEnd();
        }

        private async UniTask UpdateAsync(CancellationToken token)
        {
            Player player = World.UnitManager.FindNearby<Player>(Unit.Value.Position, p => !p.IsDead);
            if (player == null || !player.IsValid)
                return;

            WarningSound.Value.Play(player.Position);
            await UniTask.WaitForSeconds(WaitAfterWarning.Value, cancellationToken: token);

            List<(float, Vector3)> locationsWithDistance = GenerateMeteorLocations(player.Position);
            if (locationsWithDistance.Count == 0)
                return;

            float closestHit = locationsWithDistance[0].Item1;
            float mostDistantHit = locationsWithDistance[^1].Item1;
            UniTask[] meteorLaunches = new UniTask[locationsWithDistance.Count];
            for (int i = 0; i < locationsWithDistance.Count; i++)
            {
                float distance = locationsWithDistance[i].Item1;
                Vector3 location = locationsWithDistance[i].Item2;

                float distanceRatio = Mathf.InverseLerp(closestHit, mostDistantHit, distance);
                float meteorHeight = Mathf.Lerp(MeteorHeightNear.Value, MeteorHeightFar.Value, distanceRatio);
                Vector3 randomCorrection = Random.insideUnitSphere * MeteorRandomSourceRadius.Value;
                Vector3 source = location + Vector3.up * meteorHeight + new Vector3(randomCorrection.x, 0, randomCorrection.z);

                meteorLaunches[i] = LaunchMeteor(RandomUtils.Next(SpellCastDelayMin.Value, SpellCastDelayMax.Value), location, source, token);
            }

            await UniTask.WhenAll(meteorLaunches);
            await UniTask.WaitForSeconds(WaitAfterCasting.Value, cancellationToken: token);
        }

        private List<(float, Vector3)> GenerateMeteorLocations(Vector3 origin)
        {
            List<(float, Vector3)> locations = new();
            for (int circleGenerationIndex = 0; circleGenerationIndex < MinorCircles.Value; circleGenerationIndex++)
            {
                Vector3 minorCirclePoint = origin + Quaternion.Euler(0, circleGenerationIndex * 360.0f / MinorCircles.Value, 0) * Vector3.forward * MajorCircleRadius.Value;
                for (int minorCircleSpawnIndex = 0; minorCircleSpawnIndex < ProjectilesPerMinor.Value; minorCircleSpawnIndex++)
                {
                    Vector2 randomCircle = Random.insideUnitCircle * MinorCircleRadius.Value;
                    Vector3 projectileLocation = minorCirclePoint + new Vector3(randomCircle.x, 0, randomCircle.y);

                    if (NavMesh.SamplePosition(projectileLocation, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                        projectileLocation = hit.position;

                    locations.Add(((projectileLocation - Unit.Value.Position).sqrMagnitude, projectileLocation));
                }
            }
            locations.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return locations;
        }

        private async UniTask LaunchMeteor(float delay, Vector3 destination, Vector3 source, CancellationToken token)
        {
            await UniTask.WaitForSeconds(delay, cancellationToken: token);

            Unit.Value.Spells.CastSpell(
                MeteorSpellInfo.Value,
                new SpellCastingOptions(
                    new SpellExplicitTargets
                    {
                        Destination = destination,
                        Source = source
                    },
                    CastFlags.Value));
        }
    }
}
