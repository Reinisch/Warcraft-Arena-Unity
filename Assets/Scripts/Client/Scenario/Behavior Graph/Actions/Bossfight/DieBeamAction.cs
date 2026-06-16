using System;
using System.Threading;
using Client.Sound;
using Core;
using Cysharp.Threading.Tasks;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Die Beam",
        description: "Fires a beam spell from the unit towards the nearest alive player after a warning sound.",
        story: "[Unit] fires die beam with [BeamSpellInfo]",
        category: "Action/Unit",
        id: "f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1")]
    public class DieBeamAction : BehaviourGraphActionWithUtility
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<SoundEntry> WarningSound;
        [SerializeReference] public BlackboardVariable<SpellInfo> BeamSpellInfo;
        [SerializeReference] public BlackboardVariable<float> LaunchHeight = new(2f);
        [SerializeReference] public BlackboardVariable<float> TargetHeight = new(1f);
        [SerializeReference] public BlackboardVariable<float> WaitAfterWarning = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> WaitAfterCasting = new(0.2f);
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

            Vector3 source = Unit.Value.Position + Vector3.up * LaunchHeight.Value;
            Vector3 target = player.Position + Vector3.up * TargetHeight.Value;
            Vector3 direction = target - source;

            Unit.Value.Spells.CastSpell(
                BeamSpellInfo.Value,
                new SpellCastingOptions(
                    new SpellExplicitTargets { Source = source },
                    targetingSource: source,
                    targetingRotation: Quaternion.LookRotation(direction.sqrMagnitude > Mathf.Epsilon ? direction : Unit.Value.transform.forward),
                    castFlags: CastFlags.Value));

            await UniTask.WaitForSeconds(WaitAfterCasting.Value, cancellationToken: token);
        }
    }
}
