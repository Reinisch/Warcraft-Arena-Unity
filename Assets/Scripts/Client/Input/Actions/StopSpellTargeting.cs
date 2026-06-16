using UnityEngine;
using JetBrains.Annotations;
using Zenject;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Stop Spell Targeting", menuName = "Player Data/Input/Actions/Stop Spell Targeting", order = 1)]
    public class StopSpellTargeting : InputAction
    {
        [Inject] private TargetingSpellReference spellTargeting;

        public override void Execute()
        {
            spellTargeting.StopTargeting();
        }
    }
}