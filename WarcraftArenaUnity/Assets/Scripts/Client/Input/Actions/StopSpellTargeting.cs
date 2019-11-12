using UnityEngine;
using JetBrains.Annotations;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Stop Spell Targeting", menuName = "Player Data/Input/Actions/Stop Spell Targeting", order = 1)]
    public class StopSpellTargeting : InputAction
    {
        [SerializeField, UsedImplicitly] private TargetingSpellReference spellTargeting;

        public override void Execute()
        {
            spellTargeting.StopTargeting();
        }
    }
}