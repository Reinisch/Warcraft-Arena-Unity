using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Conditional Modifier - Per Resource Consumed", menuName = "Game Data/Spells/Conditional Modifiers/Per Resource Consumed", order = 1)]
    public class ConditionalModifierPerResourceConsumed : ConditionalModiferValue
    {
        [SerializeField, UsedImplicitly] private SpellPowerType powerType;
        [SerializeField, UsedImplicitly] private int maxCost;

        public override void Modify(Unit caster, Unit target, ref float value)
        {
            value *= (float)-caster.Attributes.ModifyPower(powerType, -maxCost) / maxCost;
        }
    }
}
