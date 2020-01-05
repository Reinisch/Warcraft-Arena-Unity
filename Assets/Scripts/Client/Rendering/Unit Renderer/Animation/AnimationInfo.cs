using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Animation Info", menuName = "Game Data/Animation/Animation Info", order = 1)]
    public class AnimationInfo : ScriptableUniqueInfo<AnimationInfo>
    {
        [SerializeField, UsedImplicitly] private AnimationInfoContainer container;
        [SerializeField, UsedImplicitly] private AnimationInfo fallbackAnimation;
        [SerializeField, UsedImplicitly] private string stateName;

        protected override AnimationInfo Data => this;
        protected override ScriptableUniqueInfoContainer<AnimationInfo> Container => container;

        public int StateHash { get; private set; }
        public int FallbackStateHash => fallbackAnimation.StateHash;

        protected override void OnRegister()
        {
            base.OnRegister();

            StateHash = Animator.StringToHash(stateName);
        }

        protected override void OnUnregister()
        {
            StateHash = 0;

            base.OnUnregister();
        }
    }
}
