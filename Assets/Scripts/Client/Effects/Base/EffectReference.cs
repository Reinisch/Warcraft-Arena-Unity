using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Client
{
    [CreateAssetMenu(fileName = "Effect Reference", menuName = "Game Data/Scriptable/Effects", order = 1)]
    public class EffectReference : ScriptableReference
    {
        internal class EffectContainer
        {
            private readonly List<EffectEntity> activeEffects = new List<EffectEntity>();
            private readonly List<EffectEntity> idleEffects = new List<EffectEntity>();
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

            internal void DoUpdate()
            {
                for(int i = activeEffects.Count - 1; i >= 0; i--)
                    activeEffects[i].DoUpdate();
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
                effectToPlay.transform.SetParent(parent ?? reference.containerTransform);
                idleEffects.RemoveAt(0);

                activeEffects.Add(effectToPlay);
                effectToPlay.gameObject.SetActive(true);
                effectToPlay.Play(reference.nextPlayId++);

                return effectToPlay;
            }

            internal void HandleFade(EffectEntity effectEntity)
            {
                effectEntity.transform.parent = reference.containerTransform;
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
                    effectEntity.transform.parent = reference.containerTransform;
                }
            }
            
            private void AddEffect()
            {
                EffectEntity newEffect = GameObjectPool.Take(effectSettings.Prototype, Vector3.zero, Quaternion.identity, reference.containerTransform);
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

        [SerializeField, UsedImplicitly] private List<EffectSettings> effectSettings;
        [SerializeField, UsedImplicitly] private string containerTag;

        private long nextPlayId = -1;
        private Transform containerTransform;

        protected override void OnRegistered()
        {
            containerTransform = GameObject.FindGameObjectWithTag(containerTag).transform;

            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.Initialize();
        }

        protected override void OnUnregister()
        {
            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.Deinitialize();

            containerTransform = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.EffectContainer.DoUpdate();
        }
    }
}
