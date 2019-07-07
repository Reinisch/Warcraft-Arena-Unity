using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpellHistory
    {
        private Unit unit;

        private Dictionary<int, SpellCooldown> SpellCooldowns { get; set; }

        public int GlobalCooldown { get; private set; }
        public int GlobalCooldownLeft { get; private set; }
        public float GlobalCooldownRatio => (float) GlobalCooldownLeft / GlobalCooldown;
        public bool HasGlobalCooldown => GlobalCooldownLeft > 0;

        public SpellHistory(Unit unit)
        {
            this.unit = unit;

            SpellCooldowns = new Dictionary<int, SpellCooldown>();

            if (!unit.IsOwner)
                unit.EntityState.AddCallback(nameof(unit.EntityState.GlobalCooldown), OnGlobalCooldownChanged);
        }

        internal void Dispose()
        {
            SpellCooldowns.Clear();

            unit = null;
        }

        internal void DoUpdate(int deltaTime)
        {
            if (GlobalCooldownLeft > 0)
                GlobalCooldownLeft = Mathf.Clamp(GlobalCooldownLeft - deltaTime, 0, GlobalCooldownLeft);
        }

        public bool IsReady(SpellInfo spellInfo) => !HasCooldown(spellInfo.Id);

        public bool HasCooldown(int spellInfoId) => SpellCooldowns.ContainsKey(spellInfoId);

        internal void StartGlobalCooldown(SpellInfo spellInfo)
        {
            if (spellInfo.HasAttribute(SpellExtraAttributes.DoesNotTriggerGcd))
                return;

            if (GlobalCooldownLeft > spellInfo.GlobalCooldownTime)
                return;

            GlobalCooldown = spellInfo.GlobalCooldownTime;
            GlobalCooldownLeft = GlobalCooldown;

            unit.EntityState.GlobalCooldown.CooldownTime = GlobalCooldown;
            unit.EntityState.GlobalCooldown.ServerFrame = BoltNetwork.ServerFrame;
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

        private void OnGlobalCooldownChanged()
        {
            if (!unit.IsController)
                return;

            GlobalCooldown = unit.EntityState.GlobalCooldown.CooldownTime;

            int expectedGlobalFrames = (int)(GlobalCooldown / BoltNetwork.FrameDeltaTime / 1000.0f);
            if (expectedGlobalFrames > 0)
            {
                int globalServerFrame = unit.EntityState.GlobalCooldown.ServerFrame;
                float cooldownLeftRatio = 1 - Mathf.Clamp01((float) (BoltNetwork.ServerFrame - globalServerFrame) / expectedGlobalFrames);

                GlobalCooldownLeft = Mathf.RoundToInt(cooldownLeftRatio * GlobalCooldown);
            }
            else
                GlobalCooldownLeft = 0;
        }
    }
}