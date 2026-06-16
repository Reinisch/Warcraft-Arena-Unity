using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Animation Info", menuName = "Game Data/Animation/Spell Animation Info", order = 1)]
    public class SpellAnimationInfo : ScriptableUniqueInfo<SpellAnimationInfo>
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private AnimationInfo animationInfo;

        public SpellInfo Spell => spellInfo;
        public AnimationInfo Animation => animationInfo;
    }
}