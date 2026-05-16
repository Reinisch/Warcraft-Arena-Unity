using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class HealBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private int minAmount;
        [SerializeField, UsedImplicitly] private int maxAmount;
        [SerializeField, UsedImplicitly] private float minRatio;
        [SerializeField, UsedImplicitly] private float maxRatio;

        protected override void OnStart()
        {
            base.OnStart();

            int healing = RandomUtils.Next(minAmount, maxAmount) + (int)(RandomUtils.Next(minRatio, maxRatio) * Unit.MaxHealth);
            Unit.DealHeal(Unit, healing);
        }
    }
}