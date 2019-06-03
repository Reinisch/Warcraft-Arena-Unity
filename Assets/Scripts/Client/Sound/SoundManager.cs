using Common;
using Core;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager>
{
    public new void Initialize()
    {
        base.Initialize();

        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
    }

    public new void Deinitialize()
    {
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

        base.Deinitialize();
    }

    private void OnWorldInitialized(WorldManager worldManager)
    {
        if (worldManager.HasClientLogic)
        {
            SpellManager.Instance.EventSpellCast += OnSpellManagerSpellCast;
            SpellManager.Instance.EventSpellHit += OnSpellManagerSpellHit;
        }
    }

    private void OnWorldDeinitializing(WorldManager worldManager)
    {
        if (worldManager.HasClientLogic)
        {
            SpellManager.Instance.EventSpellCast -= OnSpellManagerSpellCast;
            SpellManager.Instance.EventSpellHit -= OnSpellManagerSpellHit;
        }
    }

    private void OnSpellManagerSpellCast(Unit caster, SpellInfo spellInfo)
    {
        AudioClip castSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Cast);

        if (castSound != null)
            AudioSource.PlayClipAtPoint(castSound, caster.Position);
    }

    private void OnSpellManagerSpellHit(Unit unitTarget, SpellInfo spellInfo)
    {
        AudioClip hitSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Impact);

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, unitTarget.Position);
    }
}