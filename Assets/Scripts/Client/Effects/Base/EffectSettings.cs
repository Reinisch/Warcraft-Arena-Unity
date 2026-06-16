using JetBrains.Annotations;
using UnityEngine;
using Common;
using Zenject;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Settings", menuName = "Game Data/Visuals/Effect Settings", order = 1)]
    public class EffectSettings : ScriptableUniqueInfo<EffectSettings>
    {
        [Inject]
        private EffectReference effectModule;

        [SerializeField, UsedImplicitly] private EffectEntity prototype;
        [SerializeField, UsedImplicitly] private int maxAmount;

        internal EffectReference.EffectContainer EffectContainer { get; private set; }
        internal EffectEntity Prototype => prototype;
        internal int MaxAmount => maxAmount;

        protected override void OnRegister()
        {
            base.OnRegister();

            EffectContainer = new EffectReference.EffectContainer(this, effectModule);
        }

        protected override void OnUnregister()
        {
            EffectContainer.Dispose();
            EffectContainer = null;

            base.OnUnregister();
        }

        internal void HandleStop(EffectEntity effectEntity, bool isDestroyed)
        {
            EffectContainer.HandleStop(effectEntity, isDestroyed);
        }

        internal void HandleFade(EffectEntity effectEntity)
        {
            EffectContainer.HandleFade(effectEntity);
        }

        public EffectHandle PlayEffect(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            Assert.IsNotNull(EffectContainer, $"Effect {name} is not initialized and won't play!");
            if (EffectContainer != null)
            {
                EffectEntity newEffect = EffectContainer.Play(position, rotation, parent);
                return new EffectHandle(newEffect, newEffect?.PlayId ?? -1);
            }
            return default;
        }
    }
}
