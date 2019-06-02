using Common;
using Core;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager>
{
    public new void Initialize()
    {
        base.Initialize();
    }

    public new void Deinitialize()
    {
        base.Deinitialize();
    }

    public void InitializeWorld(WorldManager worldManager)
    {
        SpellManager.Instance.EventSpellCast += OnSpellManagerSpellCast;
        SpellManager.Instance.EventSpellHit += OnSpellManagerSpellHit;
    }

    public void DeinitializeWorld()
    {
        SpellManager.Instance.EventSpellCast -= OnSpellManagerSpellCast;
        SpellManager.Instance.EventSpellHit -= OnSpellManagerSpellHit;
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