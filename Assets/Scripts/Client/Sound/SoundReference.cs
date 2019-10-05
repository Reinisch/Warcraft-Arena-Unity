using System.Collections.Generic;
using JetBrains.Annotations;
using Common;
using Core;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Reference", menuName = "Game Data/Scriptable/Sound", order = 4)]
    public class SoundReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private string soundContainerTag;
        [SerializeField, UsedImplicitly] private UnitSoundKitContainer unitSoundKitContainer;
        [SerializeField, UsedImplicitly] private UnitSoundEmoteTypeDictionary unitSoundsByEmoteType;
        [SerializeField, UsedImplicitly] private List<SoundSettings> soundSettings;
        [SerializeField, UsedImplicitly] private List<SpellSoundSettings> spellSettings;

        private readonly Dictionary<SoundSettings, AudioSource> sourcesBySettings = new Dictionary<SoundSettings, AudioSource>();
        private readonly Dictionary<SpellInfo, SpellSoundSettings> spellSettingsByInfo = new Dictionary<SpellInfo, SpellSoundSettings>();
        private Transform soundContainer;

        public IReadOnlyDictionary<int, UnitSoundKit> UnitSoundKitsById => unitSoundKitContainer.SoundKitsById;
        public IReadOnlyDictionary<EmoteType, UnitSounds> UnitSoundByEmoteType => unitSoundsByEmoteType.ValuesByKey;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            soundContainer = GameObject.FindGameObjectWithTag(soundContainerTag).transform;

            unitSoundsByEmoteType.Populate();

            foreach (var soundSetting in soundSettings)
                sourcesBySettings[soundSetting] = ApplySettings(new GameObject(soundSetting.name).AddComponent<AudioSource>(), soundSetting);

            foreach (var spellSetting in spellSettings)
                spellSettingsByInfo[spellSetting.SpellInfo] = spellSetting;

            unitSoundKitContainer.Populate();
        }

        protected override void OnUnregister()
        {
            unitSoundKitContainer.Clear();

            foreach (var soundSourceEntry in sourcesBySettings)
                Destroy(soundSourceEntry.Value);

            unitSoundsByEmoteType.Clear();
            spellSettingsByInfo.Clear();
            sourcesBySettings.Clear();
            soundContainer = null;

            base.OnUnregister();
        }

        protected override void OnWorldInitialized(WorldManager world)
        {
            base.OnWorldInitialized(world);

            if (world.HasClientLogic)
            {
                EventHandler.RegisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.RegisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);
            }
        }

        protected override void OnWorldDeinitializing(WorldManager world)
        {
            if (world.HasClientLogic)
            {
                EventHandler.UnregisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.UnregisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);
            }

            base.OnWorldDeinitializing(world);
        }

        private void OnSpellLaunch(Unit caster, int spellId, SpellProcessingToken processingToken)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (spellSettingsByInfo.TryGetValue(spellInfo, out SpellSoundSettings spellSoundSettings))
            {
                if (spellInfo.ExplicitTargetType == SpellExplicitTargetType.Destination)
                    spellSoundSettings.PlayAtPoint(processingToken.Destination, SpellSoundEntry.UsageType.Destination);
                else
                    spellSoundSettings.PlayAtPoint(spellInfo.HasAttribute(SpellCustomAttributes.LaunchSourceIsExplicit) 
                        ? processingToken.Source
                        : caster.Position, SpellSoundEntry.UsageType.Cast);
            }
        }

        private void OnSpellHit(Unit target, int spellId)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (spellSettingsByInfo.TryGetValue(spellInfo, out SpellSoundSettings spellSoundSettings))
                spellSoundSettings.PlayAtPoint(target.Position, SpellSoundEntry.UsageType.Impact);
        }

        private AudioSource ApplySettings(AudioSource source, SoundSettings settings)
        {
            source.transform.parent = soundContainer;
            return settings.Apply(source);
        }

        public void Play(AudioClip clip, SoundSettings settings, float volumeModifier = 1.0f)
        {
            Assert.IsTrue(sourcesBySettings.ContainsKey(settings), $"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
            if (sourcesBySettings.TryGetValue(settings, out AudioSource source))
                source.PlayOneShot(clip, volumeModifier);
        }

        public void PlayAtPoint(AudioClip clip, SoundSettings settings, Vector3 position, float volumeModifier = 1.0f)
        {
            Assert.IsTrue(sourcesBySettings.ContainsKey(settings), $"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
            if (sourcesBySettings.TryGetValue(settings, out AudioSource source))
            {
                AudioSource pointSource = Instantiate(source, soundContainer);
                pointSource.transform.position = position;
                pointSource.volume = volumeModifier;
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
            spellSettings.Clear();

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(SoundSettings)}", null))
                soundSettings.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<SoundSettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(SpellSoundSettings)}", null))
                spellSettings.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<SpellSoundSettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));
        }
#endif
    }
}