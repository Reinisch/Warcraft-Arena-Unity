using System;
using Microsoft.Xna.Framework;
using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public class SpellCast : IDisposable
    {
        private bool isNotTargeted;
        public Spell Spell { get; set; }
        public ITargetable Target { get; set; }

        public TimeSpan CastTimeLeft { get; private set; }
        public TimeSpan CastTime { get; private set; }

        public SpellCast(Spell spell, SpellData spellData, ITargetable caster, ITargetable target, SpellModificationInformation spellInfo, TimeSpan latency)
        {
            Spell = spell;
            Target = target;
            if (target == null)
                isNotTargeted = true;
            else
                isNotTargeted = false;
            CastTime = TimeSpan.FromSeconds(spellData.BaseCastTime/(1+(caster.Character.Entity.HasteRating+spellInfo.HasteRatingAddMod)/100));
            CastTimeLeft = CastTime - latency;
        }

        public bool CheckInterrupt()
        {
            if (!isNotTargeted)
            {
                if (Target == null)
                    return true;
                else if (Target.NeedsDispose)
                {
                    Target = null;
                    return true;
                }
                else if (Target.Character.Entity.IsNotTargetable)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Update(Entity entity, TimeSpan elapsedGameTime)
        {
            CastTimeLeft -= elapsedGameTime;
            if (CastTimeLeft.TotalMilliseconds < 0)
            {
                CastTimeLeft = TimeSpan.Zero;
            }
            if (entity.IsNoControlable)
                return false;
            if (entity.IsSilenced)
                return false;
            return true;
        }

        public void Dispose()
        {
            Target = null;
        }
    }
}