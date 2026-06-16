using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Trigger Spell On All Players",
        description: "Triggers a spell on every player currently in the world.",
        story: "Trigger [SpellInfo] on all players",
        category: "Action/Unit",
        id: "e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b1")]
    public class TriggerSpellOnAllPlayers : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<SpellInfo> SpellInfo;

        [Inject]
        private World world;

        protected override Status OnStart()
        {
            foreach (var unit in world.UnitManager.Entities)
                if (unit is Player)
                    unit.Spells.TriggerSpell(SpellInfo.Value, unit);

            return Status.Success;
        }
    }
}
