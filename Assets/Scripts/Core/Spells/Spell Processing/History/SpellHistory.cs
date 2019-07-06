using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpellHistory
    {
        private readonly Unit owner;

        private Dictionary<int, SpellCooldown> SpellCooldowns { get; set; }

        public int GlobalCooldown { get; private set; }
        public int GlobalCooldownLeft { get; private set; }
        public float GlobalCooldownRatio => (float) GlobalCooldownLeft / GlobalCooldown;
        public bool HasGlobalCooldown => GlobalCooldownLeft > 0;

        public SpellHistory(Unit owner)
        {
            this.owner = owner;

            SpellCooldowns = new Dictionary<int, SpellCooldown>();
        }

        internal void Dispose()
        {
            SpellCooldowns.Clear();
        }

        internal void DoUpdate(int deltaTime)
        {
            if (GlobalCooldownLeft > 0)
                GlobalCooldownLeft = Mathf.Clamp(GlobalCooldownLeft - deltaTime, 0, GlobalCooldownLeft);
        }

        public bool IsReady(SpellInfo spellInfo) => !HasCooldown(spellInfo.Id);

        public bool HasCooldown(int spellInfoId) => SpellCooldowns.ContainsKey(spellInfoId);

        public void StartGlobalCooldown(SpellInfo spellInfo)
        {
            if (spellInfo.HasAttribute(SpellExtraAttributes.DoesNotTriggerGcd))
                return;

            if (GlobalCooldownLeft > spellInfo.GlobalCooldownTime)
                return;

            GlobalCooldown = spellInfo.GlobalCooldownTime;
            GlobalCooldownLeft = GlobalCooldown;
        }

        internal void HandleCooldowns(SpellInfo spellInfo)
        {
            if (!spellInfo.IsPassive())
                StartCooldown(spellInfo);
        }

        internal void StartCooldown(SpellInfo spellInfo)
        {
            float cooldown = spellInfo.CooldownTime;

            float currentTime = Time.time;

            // replace negative cooldowns by 0
            if (cooldown < 0)
                cooldown = 0;

            // no cooldown after applying spell mods
            if (cooldown == 0)
                return;

            float recTime = currentTime + cooldown;

            // self spell cooldown
            if (recTime != currentTime)
                AddCooldown(spellInfo.Id, recTime);
        }

        internal void AddCooldown(int spellId, float cooldownEnd)
        {
            SpellCooldown spellCooldown = new SpellCooldown(spellId, cooldownEnd);

            if (SpellCooldowns.ContainsKey(spellId))
                SpellCooldowns[spellId] = spellCooldown;
            else
                SpellCooldowns.Add(spellId, spellCooldown);
        }
    }
}