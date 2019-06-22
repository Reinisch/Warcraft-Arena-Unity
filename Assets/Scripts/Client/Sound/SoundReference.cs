using System.Collections.Generic;
using JetBrains.Annotations;
using Common;
using Core;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Reference", menuName = "Game Data/Scriptable/Sound", order = 4)]
    public class SoundReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private string soundContainerTag;
        [SerializeField, UsedImplicitly] private List<SoundSettings> soundSettings;

        private readonly Dictionary<SoundSettings, AudioSource> sourcesBySettings = new Dictionary<SoundSettings, AudioSource>();
        private Transform soundContainer;

        protected override void OnRegistered()
        {
            soundContainer = GameObject.FindGameObjectWithTag(soundContainerTag).transform;

            foreach (var soundSetting in soundSettings)
                sourcesBySettings[soundSetting] = ApplySettings(new GameObject(soundSetting.name).AddComponent<AudioSource>(), soundSetting);

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            foreach (var soundSourceEntry in sourcesBySettings)
                Destroy(soundSourceEntry.Value);

            sourcesBySettings.Clear();
            soundContainer = null;
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                EventHandler.RegisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, OnSpellCast);
                EventHandler.RegisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);
            }
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                EventHandler.UnregisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, OnSpellCast);
                EventHandler.UnregisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);
            }
        }

        private void OnSpellCast(Unit caster, int spellId)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            AudioClip castSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Cast);
            if (castSound != null)
                AudioSource.PlayClipAtPoint(castSound, caster.Position);
        }

        private void OnSpellHit(Unit unitTarget, int spellId)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            AudioClip hitSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Impact);
            if (hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, unitTarget.Position);
        }

        private AudioSource ApplySettings(AudioSource source, SoundSettings settings)
        {
            source.transform.parent = soundContainer;
            return settings.Apply(source);
        }

        public void Play(AudioClip clip, SoundSettings settings)
        {
            Assert.IsTrue(sourcesBySettings.ContainsKey(settings), $"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
            if (sourcesBySettings.TryGetValue(settings, out AudioSource source))
                source.PlayOneShot(clip);
        }

        public void PlayAtPoint(AudioClip clip, SoundSettings settings, Vector3 position)
        {
            Assert.IsTrue(sourcesBySettings.ContainsKey(settings), $"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
            if (sourcesBySettings.TryGetValue(settings, out AudioSource source))
            {
                AudioSource pointSource = Instantiate(source, soundContainer);
                pointSource.transform.position = position;
                pointSource.clip = clip;
                pointSource.Play();

                Destroy(pointSource.gameObject, clip.length * (Time.timeScale >= 0.01f ? Time.timeScale : 0.01f));
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Collect"), UsedImplicitly]
        private void CollectSettings()
        {
            soundSettings.Clear();

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(SoundSettings)}", null))
                soundSettings.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<SoundSettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));
        }
#endif
    }
}