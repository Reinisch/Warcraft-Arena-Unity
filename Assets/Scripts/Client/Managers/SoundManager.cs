using Core;
using UnityEngine;

public class SoundManager : SingletonGameObject<SoundManager>
{
    public override void Initialize()
    {
        base.Initialize();

        SpellManager.Instance.EventSpellCast += OnSpellManagerSpellCast;
        SpellManager.Instance.EventSpellHit += OnSpellManagerSpellHit;
    }

    public override void Deinitialize()
    {
        SpellManager.Instance.EventSpellCast -= OnSpellManagerSpellCast;
        SpellManager.Instance.EventSpellHit -= OnSpellManagerSpellHit;

        base.Deinitialize();
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