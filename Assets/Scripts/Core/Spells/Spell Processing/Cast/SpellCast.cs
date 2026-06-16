using System;

namespace Core
{
    public class SpellCast
    {
        internal enum HandleMode
        {
            Started,
            Finished
        }

        private readonly Unit caster;

        // Display-only cast driven by replication (client): the client doesn't run the cast machine, so a
        // remote unit's cast bar comes from these fields instead of a real Spell.
        private SpellInfo networkCastSpellInfo;
        private int networkCastTime;
        private int networkCastTimeLeft;

        private bool HasRealCast => Spell is { ExecutionState: SpellExecutionState.Casting };

        public bool IsCasting => HasRealCast || networkCastSpellInfo != null;

        public Spell Spell { get; private set; }
        /// <summary>The spell being cast — from the real cast machine (authority) or the replicated display cast.</summary>
        public SpellInfo CastingSpellInfo => HasRealCast ? Spell.SpellInfo : networkCastSpellInfo;
        public int CastTime => HasRealCast ? Spell.CastTime : networkCastTime;
        public int CastTimeLeft => HasRealCast ? Spell.CastTimeLeft : networkCastTimeLeft;

        public event Action EventSpellCastChanged;

        internal SpellCast(Unit caster)
        {
            this.caster = caster;
        }

        internal void Detached()
        {
            Cancel();
        }

        internal void HandleSpellCast(Spell spell, HandleMode handleMode)
        {
            switch (handleMode)
            {
                case HandleMode.Started:
                    Spell = spell;
                    break;
                case HandleMode.Finished:
                    Spell = null;
                    break;
            }

            EventSpellCastChanged?.Invoke();
        }

        public void Cancel()
        {
            if (Spell != null)
            {
                Spell.Cancel();

                HandleSpellCast(Spell, HandleMode.Finished);
            }
        }

        /// <summary>Client: begin/replace the replicated display cast (the cast bar). Ticks down locally.</summary>
        internal void SetNetworkCast(SpellInfo spellInfo, int castTime)
        {
            networkCastSpellInfo = spellInfo;
            networkCastTime = castTime;
            networkCastTimeLeft = castTime;
            EventSpellCastChanged?.Invoke();
        }

        /// <summary>Client: end the replicated display cast (completed/interrupted server-side).</summary>
        internal void ClearNetworkCast()
        {
            if (networkCastSpellInfo == null)
                return;

            networkCastSpellInfo = null;
            networkCastTime = 0;
            networkCastTimeLeft = 0;
            EventSpellCastChanged?.Invoke();
        }

        /// <summary>Client: tick the display cast bar down between replicated updates.</summary>
        internal void DoNetworkUpdate(int deltaTime)
        {
            if (networkCastSpellInfo == null)
                return;

            networkCastTimeLeft -= deltaTime;
            if (networkCastTimeLeft <= 0)
                ClearNetworkCast();
        }
    }
}