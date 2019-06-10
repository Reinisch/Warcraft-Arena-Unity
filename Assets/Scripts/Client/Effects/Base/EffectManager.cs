using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Client
{
    public class EffectManager : MonoBehaviour
    {
        internal class EffectContainer
        {
            private readonly List<EffectEntity> activeEffects = new List<EffectEntity>();
            private readonly List<EffectEntity> idleEffects = new List<EffectEntity>();
            private readonly EffectSettings effectSettings;
            private readonly EffectManager effectManager;

            internal EffectContainer(EffectSettings effectSettings, EffectManager effectManager)
            {
                this.effectSettings = effectSettings;
                this.effectManager = effectManager;

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
                effectToPlay.transform.SetParent(parent ?? effectManager.effectOrigin);
                idleEffects.RemoveAt(0);

                activeEffects.Add(effectToPlay);
                effectToPlay.Play(effectManager.nextPlayId++);
                effectToPlay.gameObject.SetActive(true);

                return effectToPlay;
            }

            internal void Stop(EffectEntity effectEntity, bool isDestroyed)
            {
                if (isDestroyed)
                {
                    if (effectEntity.State == EffectState.Active)
                        activeEffects.Remove(effectEntity);
                    else if (effectEntity.State == EffectState.Idle)
                        idleEffects.Remove(effectEntity);

                    effectEntity.Deinitialize();
                    GameObjectPool.Return(effectEntity, true);
                }
                else
                {
                    Assert.IsTrue(effectEntity.State == EffectState.Active, $"Stopped effect with invalid state: {effectEntity.State} at: {effectEntity.GetPath()}!");
                    if (effectEntity.State == EffectState.Active)
                    {
                        activeEffects.Remove(effectEntity);
                        idleEffects.Add(effectEntity);
                    }

                    effectEntity.gameObject.SetActive(false);
                }
            }
            
            private void AddEffect()
            {
                EffectEntity newEffect = GameObjectPool.Take(effectSettings.Prototype, Vector3.zero, Quaternion.identity, effectManager.effectOrigin);
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
        [SerializeField, UsedImplicitly] private Transform effectOrigin;

        private long nextPlayId = -1;

        public void Initialize()
        {
            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.Initialize(this);
        }

        public void Deinitialize()
        {
            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.Deinitialize();
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (EffectSettings effectSetting in effectSettings)
                effectSetting.EffectContainer.DoUpdate();
        }
    }
}
