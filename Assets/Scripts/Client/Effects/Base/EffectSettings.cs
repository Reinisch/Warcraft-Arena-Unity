using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Settings", menuName = "Game Data/Visuals/Effect Settings", order = 1)]
    public class EffectSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private EffectReference reference;
        [SerializeField, UsedImplicitly] private EffectEntity prototype;
        [SerializeField, UsedImplicitly] private int maxAmount;

        internal EffectReference.EffectContainer EffectContainer { get; private set; }
        internal EffectEntity Prototype => prototype;
        internal int MaxAmount => maxAmount;

        internal void Initialize()
        {
            EffectContainer = new EffectReference.EffectContainer(this, reference);
        }

        internal void Deinitialize()
        {
            EffectContainer.Dispose();
            EffectContainer = null;
        }

        internal void HandleStop(EffectEntity effectEntity, bool isDestroyed)
        {
            EffectContainer.HandleStop(effectEntity, isDestroyed);
        }

        internal void HandleFade(EffectEntity effectEntity)
        {
            EffectContainer.HandleFade(effectEntity);
        }

        public IEffectEntity PlayEffect(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return PlayEffect(position, rotation, out _, parent);
        }

        public IEffectEntity PlayEffect(Vector3 position, Quaternion rotation, out long playId, Transform parent = null)
        {
            Assert.IsNotNull(EffectContainer, $"Effect {name} is not initialized and won't play!");

            if (EffectContainer != null)
            {
                EffectEntity newEffect = EffectContainer.Play(position, rotation, parent);
                playId = newEffect?.PlayId ?? -1;
                return newEffect;
            }

            playId = -1;
            return null;
        }
    }
}
