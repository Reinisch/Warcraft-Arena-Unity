using Common;
using Core;
using JetBrains.Annotations;
using System.Collections.Generic;
using Client.Sound;
using UnityEngine;
using Zenject;

namespace Client
{
    public class SoundReference : SoundModule
    {
        [Inject] private BalanceReference balance;
        [Inject] private EventBus eventBus;

        [SerializeField, UsedImplicitly] private UnitSoundKitContainer unitSoundKits;
        [SerializeField, UsedImplicitly] private SpellSoundInfoContainer spellSounds;
        [SerializeField, UsedImplicitly] private UnitSoundEmoteTypeDictionary unitSoundsByEmoteType;

        private readonly Dictionary<SpellInfo, SpellSoundInfo> spellSoundInfos = new();

        public IReadOnlyDictionary<EmoteType, UnitSounds> UnitSoundByEmoteType => unitSoundsByEmoteType.ValuesByKey;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            spellSounds.Register();
            foreach (SpellSoundInfo spellSetting in spellSounds.ItemList)
                spellSoundInfos[spellSetting.SpellInfo] = spellSetting;

            unitSoundsByEmoteType.Register();
            unitSoundKits.Register();

            eventBus.RegisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
            eventBus.RegisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);
        }

        protected override void OnUnregister()
        {
            eventBus.UnregisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
            eventBus.UnregisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

            unitSoundKits.Unregister();
            unitSoundsByEmoteType.Unregister();
            spellSoundInfos.Clear();
            spellSounds.Unregister();

            base.OnUnregister();
        }
        
        protected override void QueueForInject(DiContainer container)
        {
            base.QueueForInject(container);

            unitSoundKits.QueueForInject(container);
            spellSounds.QueueForInject(container);
        }

        public void OnProjectileImpact(Projectile projectile)
        {
            if (!balance.SpellInfosById.TryGetValue(projectile.HitSpell.Id, out SpellInfo spellInfo))
                return;

            if (spellSoundInfos.TryGetValue(spellInfo, out SpellSoundInfo spellSoundSettings))
                spellSoundSettings.PlayAtPoint(projectile.Transform.position, SpellSoundEntry.UsageType.Impact);
        }

        private void OnSpellLaunch(Unit caster, int spellId, SpellProcessingToken processingToken)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (spellSoundInfos.TryGetValue(spellInfo, out SpellSoundInfo spellSoundSettings))
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

            if (spellSoundInfos.TryGetValue(spellInfo, out SpellSoundInfo spellSoundSettings))
                spellSoundSettings.PlayAtPoint(target.Position, SpellSoundEntry.UsageType.Impact);
        }
    }
}