using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Animation Info Container", menuName = "Game Data/Containers/Spell Animation Info", order = 1)]
    public class SpellAnimationInfoContainer : ScriptableUniqueInfoContainer<SpellAnimationInfo>
    {
        [SerializeField, UsedImplicitly] private SpellInfoContainer spellContainer;
        [SerializeField, UsedImplicitly] private AnimationInfo defaultAnimation;

        public AnimationInfo DefaultAnimation => defaultAnimation;

        // The spellId→animation lookup lives on RenderingReference (a MonoBehaviour whose state resets each
        // play session). A ScriptableObject must not retain runtime lookup state between editor/MPPM
        // sessions; the serialized config (default animation) stays here.
    }
}
