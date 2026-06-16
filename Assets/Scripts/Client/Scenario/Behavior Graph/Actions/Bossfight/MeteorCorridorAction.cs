using Common;
using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Client.Sound;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Meteor Corridor",
        description: "Teleports the player into a meteor corridor, fires a barrage of meteor spells, and ends when the player escapes.",
        story: "[Unit] performs meteor corridor at [MeteorLocations] with attacks [attackEntries]",
        category: "Action/Unit",
        id: "c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4")]
    public class MeteorCorridorAction : BehaviourGraphActionWithUtility
    {
        [Serializable]
        public class AttackEntry
        {
            public Vector3 LaunchOffset;
            public Vector3 LaunchAngles;
            public float Cooldown;
        }

        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<List<GameObject>> MeteorLocations;
        [SerializeReference] public BlackboardVariable<SoundEntry> StartSound;
        [SerializeReference] public BlackboardVariable<SoundEntry> CorridorSpawnSound;
        [SerializeReference] public BlackboardVariable<EffectSettings> CorridorEffect;
        [SerializeReference] public BlackboardVariable<SpellInfo> MeteorSpellInfo;
        [SerializeReference] public BlackboardVariable<SpellInfo> ControlSpellInfo;
        [SerializeReference] public BlackboardVariable<float> TimeToSpawnCorridor;
        [SerializeReference] public BlackboardVariable<float> TimeToWaitAfterSpawn;
        [SerializeReference] public BlackboardVariable<float> TimeToWaitAfterAttack;
        [SerializeReference] public BlackboardVariable<Vector3> PlayerTeleportOffset;
        [SerializeReference] public BlackboardVariable<float> EscapeRangeMin;
        [SerializeReference] public BlackboardVariable<float> EscapeRangeMax;
        [SerializeReference] public BlackboardVariable<SpellCastFlags> CastFlags;
        [SerializeReference] public BlackboardVariable<MeteorCorridorActionSettings> Settings;

        [Inject]
        private CameraReference cameraModule;

        private bool isRunning;
        private Transform meteorTarget;
        private CancellationTokenSource cancellationSource;
        private EffectHandle corridorEffectHandle;

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

            corridorEffectHandle.Stop();
            meteorTarget = null;

            base.OnEnd();
        }

        private async UniTask UpdateAsync(CancellationToken token)
        {
            meteorTarget = RandomUtils.GetRandomElement(MeteorLocations.Value).transform;

            Player player = World.UnitManager.FindNearby<Player>(Unit.Value.Position, p => !p.IsDead);
            if (player == null || !player.IsValid)
                return;

            player.Teleport(meteorTarget.TransformPoint(PlayerTeleportOffset.Value));
            cameraModule.WarcraftCamera.LookAt(meteorTarget.position);

            Unit.Value.Spells.CastSpell(
                ControlSpellInfo.Value,
                new SpellCastingOptions(new SpellExplicitTargets(player), CastFlags.Value));

            await UniTask.WaitForSeconds(TimeToSpawnCorridor.Value, cancellationToken: token);

            CorridorSpawnSound.Value.Play(player.Position);
            corridorEffectHandle = CorridorEffect.Value.PlayEffect(meteorTarget.position, meteorTarget.rotation);

            Unit.Value.Spells.CastSpell(
                ControlSpellInfo.Value,
                new SpellCastingOptions(new SpellExplicitTargets(player), CastFlags.Value));

            await UniTask.WaitForSeconds(TimeToWaitAfterSpawn.Value, cancellationToken: token);

            StartSound.Value.Play(player.Position);

            using var attackCancellationSource = new CancellationTokenSource();
            using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(attackCancellationSource.Token, token);
            UniTask attackSequence = ExecuteAttackSequenceAsync(linkedCancellationSource.Token).SuppressCancellationThrow();
            UniTask escapeSequence = WaitUntilPlayerEscaped(player, linkedCancellationSource.Token).SuppressCancellationThrow();
            await UniTask.WhenAny(attackSequence, escapeSequence);
            attackCancellationSource.Cancel();
            token.ThrowIfCancellationRequested();

            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            await UniTask.WaitForSeconds(TimeToWaitAfterAttack.Value, cancellationToken: token);

            corridorEffectHandle.Fade();
            CorridorSpawnSound.Value.Play(player.Position);

            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        }

        private async UniTask ExecuteAttackSequenceAsync(CancellationToken attackCancellationToken)
        {
            for (var index = 0; index < Settings.Value.AttackEntries.Count; index++)
            {
                AttackEntry attackEntry = Settings.Value.AttackEntries[index];
                Vector3 source = meteorTarget.TransformPoint(attackEntry.LaunchOffset);

                Unit.Value.Spells.CastSpell(
                    MeteorSpellInfo.Value,
                    new SpellCastingOptions(
                        new SpellExplicitTargets { Source = source },
                        targetingSource: source,
                        targetingRotation: meteorTarget.rotation * Quaternion.Euler(attackEntry.LaunchAngles),
                        castFlags: CastFlags.Value));

                if (index != Settings.Value.AttackEntries.Count - 1)
                    await UniTask.WaitForSeconds(attackEntry.Cooldown, cancellationToken: attackCancellationToken);
            }
        }

        private async UniTask WaitUntilPlayerEscaped(Unit player, CancellationToken escapeCancellationToken)
        {
            await UniTask.WaitUntil(EscapePredicate, cancellationToken: escapeCancellationToken);

            bool EscapePredicate()
            {
                if (!player.IsValid || player.IsDead)
                    return false;

                if (Vector3.Dot(meteorTarget.forward, player.Position - meteorTarget.position) < 0)
                    return true;

                float distance = Vector3.Distance(meteorTarget.position, player.Position);
                return distance > EscapeRangeMax.Value || distance < EscapeRangeMin.Value;
            }
        }
    }
}
