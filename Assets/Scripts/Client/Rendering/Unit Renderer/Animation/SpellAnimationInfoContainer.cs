using System.Collections.Generic;
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
        [SerializeField, UsedImplicitly] private List<SpellAnimationInfo> animationInfos;

        private readonly Dictionary<int, AnimationInfo> animationInfoBySpellId = new Dictionary<int, AnimationInfo>();

        protected override List<SpellAnimationInfo> Items => animationInfos;

        public override void Register()
        {
            base.Register();

            foreach (var spellAnimation in animationInfos)
                animationInfoBySpellId.Add(spellAnimation.Spell.Id, spellAnimation.Animation);
        }

        public override void Unregister()
        {
            animationInfoBySpellId.Clear();

            base.Unregister();
        }

        public AnimationInfo FindAnimation(SpellInfo spellInfo)
        {
            if (animationInfoBySpellId.TryGetValue(spellInfo.Id, out AnimationInfo animationInfo))
                return animationInfo;

            return defaultAnimation;
        }
    }
}