using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Overlay Settings", menuName = "Game Data/Interface/Spell Overlay Settings", order = 2)]
    public class SpellOverlaySettings : ScriptableUniqueInfo<SpellOverlaySettings>
    {
        [SerializeField, UsedImplicitly] private SpellOverlay prototype;
        [SerializeField, UsedImplicitly] private AuraInfo triggerAura;

        public AuraInfo TriggerAura => triggerAura;
        public SpellOverlay Prototype => prototype;
    }
}
