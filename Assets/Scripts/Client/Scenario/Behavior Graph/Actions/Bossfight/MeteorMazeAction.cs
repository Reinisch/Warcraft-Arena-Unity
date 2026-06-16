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

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Meteor Maze",
        description: "Teleports the player to a meteor location, spawns maze rings, then fires a meteor when they escape or time runs out.",
        story: "[Unit] performs meteor maze at [MeteorLocations]",
        category: "Action/Unit",
        id: "d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5")]
    public class MeteorMazeAction : BehaviourGraphActionWithUtility
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<List<GameObject>> MeteorLocations;
        [SerializeReference] public BlackboardVariable<SoundEntry> StartSound;
        [SerializeReference] public BlackboardVariable<SoundEntry> MazeSpawnSound;
        [SerializeReference] public BlackboardVariable<SoundEntry> EscapeSound;
        [SerializeReference] public BlackboardVariable<SpellInfo> MeteorSpellInfo;
        [SerializeReference] public BlackboardVariable<SpellInfo> AfterShockSpellInfo;
        [SerializeReference] public BlackboardVariable<SpellInfo> ControlSpellInfo;
        [SerializeReference] public BlackboardVariable<float> MeteorHeight = new(30f);
        [SerializeReference] public BlackboardVariable<float> EscapeRange;
        [SerializeReference] public BlackboardVariable<float> TimeToEscape;
        [SerializeReference] public BlackboardVariable<float> TimeToSpawnRing;
        [SerializeReference] public BlackboardVariable<float> TimeToWaitAfterRings;
        [SerializeReference] public BlackboardVariable<SpellCastFlags> CastFlags;
        [SerializeReference] public BlackboardVariable<MeteorMazeActionSettings> Settings;

        private bool isRunning;
        private Transform meteorTarget;
        private CancellationTokenSource cancellationSource;
        private readonly List<EffectHandle> mazeRingEffectEntities = new();

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

            mazeRingEffectEntities.ForEach(handle => handle.Stop());
            mazeRingEffectEntities.Clear();
            meteorTarget = null;

            base.OnEnd();
        }

        private async UniTask UpdateAsync(CancellationToken token)
        {
            meteorTarget = RandomUtils.GetRandomElement(MeteorLocations.Value).transform;
            Player player = World.UnitManager.FindNearby<Player>(meteorTarget.position, p => !p.IsDead);
            if (player == null || !player.IsValid)
                return;

            player.Teleport(meteorTarget.position);
            Unit.Value.Spells.CastSpell(
                ControlSpellInfo.Value,
                new SpellCastingOptions(new SpellExplicitTargets(player), CastFlags.Value));

            await UniTask.WaitForSeconds(0.2f, cancellationToken: token);

            foreach (var mazeSettings in Settings.Value.MazeRingEffects)
            {
                Unit.Value.Spells.CastSpell(
                    ControlSpellInfo.Value,
                    new SpellCastingOptions(new SpellExplicitTargets(player), CastFlags.Value));

                await UniTask.WaitForSeconds(TimeToSpawnRing.Value, cancellationToken: token);

                MazeSpawnSound.Value.Play(meteorTarget.position);

                EffectHandle newRing = mazeSettings.PlayEffect(meteorTarget.position, meteorTarget.rotation * Quaternion.Euler(0, RandomUtils.Next(0, 360), 0));
                if (newRing.IsValid)
                    mazeRingEffectEntities.Add(newRing);
            }

            Unit.Value.Spells.CastSpell(
                ControlSpellInfo.Value,
                new SpellCastingOptions(new SpellExplicitTargets(player), CastFlags.Value));

            await UniTask.WaitForSeconds(TimeToWaitAfterRings.Value, cancellationToken: token);

            StartSound.Value.Play(player.Position);

            UniTask playerDone = UniTask.WaitUntil(() =>
                    !player.IsValid ||
                    player.IsDead ||
                    Vector3.Distance(meteorTarget.position, player.Position) > EscapeRange.Value,
                cancellationToken: token);

            UniTask timeOut = UniTask.WaitForSeconds(TimeToEscape.Value, cancellationToken: token);

            await UniTask.WhenAny(playerDone, timeOut);

            Unit.Value.Spells.CastSpell(
                MeteorSpellInfo.Value,
                new SpellCastingOptions(
                    new SpellExplicitTargets
                    {
                        Destination = meteorTarget.position,
                        Source = meteorTarget.position + Vector3.up * MeteorHeight.Value
                    },
                    CastFlags.Value));

            if (AfterShockSpellInfo.Value != null)
            {
                Unit.Value.Spells.CastSpell(
                    AfterShockSpellInfo.Value,
                    new SpellCastingOptions(
                        new SpellExplicitTargets
                        {
                            Destination = meteorTarget.position,
                            Source = meteorTarget.position + Vector3.up * MeteorHeight.Value
                        },
                        CastFlags.Value));
            }

            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            mazeRingEffectEntities.ForEach(handle => handle.Fade());

            EscapeSound.Value.Play(player.Position);

            await UniTask.WaitForSeconds(1f, cancellationToken: token);
        }
    }
}
