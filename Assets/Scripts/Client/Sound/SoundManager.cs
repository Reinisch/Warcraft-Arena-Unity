using Common;
using Core;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void Initialize()
    {
        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
    }

    public void Deinitialize()
    {
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
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
        if (!BalanceManager.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
            return;

        AudioClip castSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Cast);
        if (castSound != null)
            AudioSource.PlayClipAtPoint(castSound, caster.Position);
    }

    private void OnSpellHit(Unit unitTarget, int spellId)
    {
        if (!BalanceManager.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
            return;

        AudioClip hitSound = spellInfo.SoundSettings.FindSound(SpellSoundEntry.UsageType.Impact);
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, unitTarget.Position);
    }
}