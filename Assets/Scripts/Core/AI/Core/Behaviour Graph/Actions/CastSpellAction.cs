using System;
using Common;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Cast Spell",
        description: "Casts a spell from the unit, optionally finding and predicting a nearby or random player target.",
        story: "[Unit] casts [SpellInfo]",
        category: "Action/Unit",
        id: "b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e8")]
    public class CastSpellAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<SpellInfo> SpellInfo;
        [SerializeReference] public BlackboardVariable<bool> FindTargetNearby;
        [SerializeReference] public BlackboardVariable<bool> OnlyTargetAlive;
        [SerializeReference] public BlackboardVariable<bool> PredictDestination;
        [SerializeReference] public BlackboardVariable<float> PredictMinRatio = new(0.2f);
        [SerializeReference] public BlackboardVariable<float> PredictMaxRatio = new(0.9f);
        [SerializeReference] public BlackboardVariable<bool> TargetRequired;
        [SerializeReference] public BlackboardVariable<SpellCastFlags> CastFlags;

        [Inject]
        private World world;

        protected override Status OnStart()
        {
            if (!SelectTarget(out SpellExplicitTargets explicitTargets))
                return Status.Success;

            Unit.Value.Spells.CastSpell(
                SpellInfo.Value,
                new SpellCastingOptions(explicitTargets, CastFlags.Value));
            
            return Status.Success;
        }

        private bool SelectTarget(out SpellExplicitTargets explicitTargets)
        {
            if (!TargetRequired.Value)
            {
                explicitTargets = new SpellExplicitTargets();
                return true;
            }

            Player player = FindTargetNearby.Value
                ? world.UnitManager.FindNearby<Player>(Unit.Value.Position, TargetPredicate)
                : world.UnitManager.FindRandom<Player>(TargetPredicate);

            if (player == null)
            {
                explicitTargets = null;
                return false;
            }

            if (PredictDestination.Value)
            {
                float distance = Mathf.Clamp(Vector3.Distance(Unit.Value.Position, player.Position), StatUtils.DefaultCombatReach, float.MaxValue);
                float delay = SpellInfo.Value.Delay > 0 ? SpellInfo.Value.Delay / 1000.0f : distance / SpellInfo.Value.Speed;

                Vector3 predictionDelta = player.CharacterController.Velocity * delay;
                predictionDelta.y = 0;

                if (PredictMinRatio.Value > 0 && PredictMaxRatio.Value > 0)
                    predictionDelta *= RandomUtils.Next(PredictMinRatio.Value, PredictMaxRatio.Value);

                Vector3 predictedTarget = player.Position + predictionDelta;

                if (NavMesh.SamplePosition(predictedTarget, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                {
                    explicitTargets = new SpellExplicitTargets { Destination = hit.position };
                    return true;
                }
            }

            explicitTargets = new SpellExplicitTargets(player);
            return true;
        }

        private bool TargetPredicate(Player player)
        {
            if (OnlyTargetAlive.Value && player.IsDead)
                return false;

            return true;
        }
    }
}
