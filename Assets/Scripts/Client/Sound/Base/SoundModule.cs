using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client.Sound
{
    public abstract class SoundModule : ScriptableReference
    {
        [Inject] private SoundPlayController playController;

        [SerializeField, UsedImplicitly] private SoundEntryContainer soundEntries;
        [SerializeField, UsedImplicitly] private SoundGroupSettingsContainer soundGroups;

        // Runtime source pool (one prototype AudioSource per group). Lives here on the MonoBehaviour, not on
        // the ScriptableObject container, so its runtime GameObjects don't leak between play sessions.
        private readonly Dictionary<SoundGroupSettings, SoundHandleComponent> sourcesBySettings = new();

        internal SoundEntryContainer SoundEntries => soundEntries;
        internal Transform Container => transform;

        protected override void OnRegistered()
        {
            soundEntries.Register();
            soundGroups.Register();

            foreach (SoundGroupSettings soundSetting in soundGroups.ItemList)
            {
                GameObject prototype = new GameObject(soundSetting.name);
                AudioSource source = soundSetting.Apply(prototype.AddComponent<AudioSource>());
                prototype.transform.parent = Container;
                sourcesBySettings[soundSetting] = prototype.AddComponent<SoundHandleComponent>().Create(source);
            }

            playController.Register();
        }

        protected override void OnUnregister()
        {
            playController.Unregister();

            foreach (KeyValuePair<SoundGroupSettings, SoundHandleComponent> entry in sourcesBySettings)
                Destroy(entry.Value);
            sourcesBySettings.Clear();

            soundGroups.Unregister();
            soundEntries.Unregister();
        }

        protected override void QueueForInject(DiContainer container)
        {
            soundGroups.QueueForInject(container);
            soundEntries.QueueForInject(container);
        }

        internal SoundHandleComponent PlayAtPoint(
            AudioClip clip,
            SoundGroupSettings settings,
            Vector3 position,
            SoundPlayHandle playHandle,
            Transform parent = null,
            float volumeModifier = 1.0f,
            float delay = 0.0f,
            float speed = 1.0f,
            float startTime = 0.0f)
        {
            if (sourcesBySettings.TryGetValue(settings, out SoundHandleComponent prototype))
            {
                SoundHandleComponent handleComponent = GameObjectPool.Take(prototype, position, Quaternion.identity, Container);
                AudioSource pointSource = handleComponent.Source;

                pointSource.transform.position = position;
                pointSource.volume = volumeModifier;
                pointSource.clip = clip;
                pointSource.pitch = speed;
                pointSource.time = startTime;

                if (delay > 0)
                    pointSource.PlayDelayed(delay);
                else
                    pointSource.Play();

                handleComponent.Play(playHandle);

                if (playHandle.ScheduleRelease)
                    playController.ScheduleRelease(playHandle.PlayId, delay + clip.length / speed);

                if (parent != null)
                    pointSource.transform.SetParent(parent);

                return handleComponent;
            }

            Assert.Fail($"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
            return null;
        }
    }
}