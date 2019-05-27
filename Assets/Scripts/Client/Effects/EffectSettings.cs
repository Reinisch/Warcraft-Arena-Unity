using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Client.Effects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Settings", menuName = "Game Data/Visual Effects/Effect Settings", order = 1)]
    public class EffectSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private EffectEntity prototype;
        [SerializeField, UsedImplicitly] private int maxAmount;

        internal EffectManager.EffectContainer EffectContainer { get; private set; }
        internal EffectEntity Prototype => prototype;
        internal int MaxAmount => maxAmount;

        internal void Initialize(EffectManager effectManager)
        {
            EffectContainer = new EffectManager.EffectContainer(this, effectManager);
        }

        internal void Deinitialize()
        {
            EffectContainer.Dispose();
            EffectContainer = null;
        }

        internal void StopEffect(EffectEntity effectEntity, bool isDestroyed)
        {
            EffectContainer.StopEffect(effectEntity, isDestroyed);
        }

        public EffectEntity PlayEffect(Vector3 position, Quaternion rotation, Transform parent)
        {
            Assert.IsNotNull(EffectContainer, $"Effect {name} is not initialized and won't play!");

            return EffectContainer?.Play(position, rotation, parent);
        }
    }
}
