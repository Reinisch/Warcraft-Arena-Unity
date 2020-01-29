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
        [SerializeField, UsedImplicitly] private string soundContainerTag;
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private UnitSoundKitContainer unitSoundKits;
        [SerializeField, UsedImplicitly] private SpellSoundInfoContainer spellSounds;
        [SerializeField, UsedImplicitly] private SoundGroupSettingsContainer soundGroups;
        [SerializeField, UsedImplicitly] private UnitSoundEmoteTypeDictionary unitSoundsByEmoteType;

        public Transform Container { get; private set; }
        public IReadOnlyDictionary<EmoteType, UnitSounds> UnitSoundByEmoteType => unitSoundsByEmoteType.ValuesByKey;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            Container = GameObject.FindGameObjectWithTag(soundContainerTag).transform;

            soundGroups.Register();
            spellSounds.Register();
            unitSoundsByEmoteType.Register();
            unitSoundKits.Register();
        }

        protected override void OnUnregister()
        {
            unitSoundKits.Unregister();
            unitSoundsByEmoteType.Unregister();

            spellSounds.Unregister();
            soundGroups.Unregister();

            Container = null;

            base.OnUnregister();
        }

        protected override void OnWorldStateChanged(World world, bool created)
        {
            if (created)
            {
                base.OnWorldStateChanged(world, true);

                EventHandler.RegisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.RegisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);
            }
            else
            {
                EventHandler.UnregisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.UnregisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

                base.OnWorldStateChanged(world, false);
            }
        }

        private void OnSpellLaunch(Unit caster, int spellId, SpellProcessingToken processingToken)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (spellSounds.SoundInfos.TryGetValue(spellInfo, out SpellSoundInfo spellSoundSettings))
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

            if (spellSounds.SoundInfos.TryGetValue(spellInfo, out SpellSoundInfo spellSoundSettings))
                spellSoundSettings.PlayAtPoint(target.Position, SpellSoundEntry.UsageType.Impact);
        }

        public void Play(AudioClip clip, SoundGroupSettings settings, float volumeModifier = 1.0f)
        {
            if (soundGroups.TryGetSource(settings, out AudioSource source))
                source.PlayOneShot(clip, volumeModifier);
            else
                Assert.Fail($"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
        }

        public void PlayAtPoint(AudioClip clip, SoundGroupSettings settings, Vector3 position, float volumeModifier = 1.0f)
        {
            if (soundGroups.TryGetSource(settings, out AudioSource source))
            {
                AudioSource pointSource = Instantiate(source, Container);
                pointSource.transform.position = position;
                pointSource.volume = volumeModifier;
                pointSource.clip = clip;
                pointSource.Play();

                Destroy(pointSource.gameObject, clip.length * (Time.timeScale >= 0.01f ? Time.timeScale : 0.01f));
            }
            else
                Assert.Fail($"Sound settings {settings.name} are not initialized and clip: {clip.name} won't play!");
        }
    }
}