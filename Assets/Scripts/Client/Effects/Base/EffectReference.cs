using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;
using Zenject;

namespace Client
{
    public class EffectReference : ScriptableReference
    {
        internal class EffectContainer
        {
            private readonly List<EffectEntity> activeEffects = new();
            private readonly List<EffectEntity> idleEffects = new();
            private readonly EffectSettings effectSettings;
            private readonly EffectReference reference;

            internal EffectContainer(EffectSettings effectSettings, EffectReference reference)
            {
                this.effectSettings = effectSettings;
                this.reference = reference;

                AddEffects(effectSettings.MaxAmount);
            }

            internal void Dispose()
            {
                foreach (EffectEntity activeEffect in activeEffects)
                {
                    activeEffect.Deinitialize();
                    GameObjectPool.Return(activeEffect, false);
                }

                foreach (EffectEntity idleEffect in idleEffects)
                {
                    idleEffect.Deinitialize();
                    GameObjectPool.Return(idleEffect, false);
                }

                idleEffects.Clear();
                activeEffects.Clear();
            }

            internal void DoUpdate(float deltaTime)
            {
                for(int i = activeEffects.Count - 1; i >= 0; i--)
                    activeEffects[i].DoUpdate(deltaTime);
            }

            internal EffectEntity Play(Vector3 position, Quaternion rotation, Transform parent)
            {
                if (activeEffects.Count >= effectSettings.MaxAmount)
                    return null;

                if (idleEffects.Count == 0)
                    AddEffects(1);

                var effectToPlay = idleEffects[0];
                effectToPlay.transform.position = position;
                effectToPlay.transform.rotation = rotation;
                effectToPlay.transform.SetParent(parent ?? reference.EffectsRoot);
                effectToPlay.transform.localScale = Vector3.one;

                idleEffects.RemoveAt(0);

                activeEffects.Add(effectToPlay);
                effectToPlay.gameObject.SetActive(true);
                effectToPlay.Play(reference.nextPlayId++);

                return effectToPlay;
            }

            internal void HandleFade(EffectEntity effectEntity)
            {
                effectEntity.transform.parent = reference.EffectsRoot;
            }

            internal void HandleStop(EffectEntity effectEntity, bool isDestroyed)
            {
                if (isDestroyed)
                {
                    if (effectEntity.State.IsPlaying())
                        activeEffects.Remove(effectEntity);
                    else if (effectEntity.State.IsIdle())
                        idleEffects.Remove(effectEntity);

                    effectEntity.Deinitialize();
                    GameObjectPool.Return(effectEntity, true);
                }
                else
                {
                    Assert.IsTrue(effectEntity.State.IsPlaying(), $"Stopped effect with invalid state: {effectEntity.State} at: {effectEntity.GetPath()}!");
                    if (effectEntity.State.IsPlaying())
                    {
                        activeEffects.Remove(effectEntity);
                        idleEffects.Add(effectEntity);
                    }

                    effectEntity.gameObject.SetActive(false);
                    effectEntity.transform.parent = reference.EffectsRoot;
                }
            }
            
            private void AddEffect()
            {
                EffectEntity newEffect = GameObjectPool.Take(effectSettings.Prototype, Vector3.zero, Quaternion.identity, reference.EffectsRoot);
                newEffect.Initialize(effectSettings);
                idleEffects.Add(newEffect);

                newEffect.gameObject.SetActive(false);
            }

            private void AddEffects(int count)
            {
                for (int i = 0; i < count; i++)
                    AddEffect();
            }
        }

        [SerializeField, UsedImplicitly]
        private EffectSettingsContainer effectsContainer;

        private long nextPlayId = -1;
        internal Transform EffectsRoot => transform;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            effectsContainer.Register();
        }

        protected override void OnUnregister()
        {
            effectsContainer.Unregister();

            base.OnUnregister();
        }

        protected override void QueueForInject(DiContainer container)
        {
            base.QueueForInject(container);

            effectsContainer.QueueForInject(container);
        }

        protected override void OnUpdate(float deltaTime)
        {
            IReadOnlyList<EffectSettings> items = effectsContainer.ItemList;
            for (int i = 0; i < items.Count; i++)
                items[i].EffectContainer.DoUpdate(deltaTime);
        }
    }
}
