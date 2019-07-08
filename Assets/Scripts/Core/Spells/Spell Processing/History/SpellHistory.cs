using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public class SpellHistory
    {
        private readonly Dictionary<int, SpellCooldown> spellCooldownsById = new Dictionary<int, SpellCooldown>();
        private readonly List<SpellCooldown> spellCooldowns = new List< SpellCooldown>();
        private Unit unit;

        public int GlobalCooldown { get; private set; }
        public int GlobalCooldownLeft { get; private set; }
        public bool HasGlobalCooldown => GlobalCooldownLeft > 0;

        public SpellHistory(Unit unit)
        {
            this.unit = unit;

            if (!unit.IsOwner)
                unit.EntityState.AddCallback(nameof(unit.EntityState.GlobalCooldown), OnGlobalCooldownChanged);
        }

        internal void Dispose()
        {
            spellCooldowns.Clear();
            spellCooldownsById.Clear();

            unit = null;
        }

        internal void DoUpdate(int deltaTime)
        {
            if (GlobalCooldownLeft > 0)
                GlobalCooldownLeft = Mathf.Clamp(GlobalCooldownLeft - deltaTime, 0, GlobalCooldownLeft);

            foreach (SpellCooldown cooldown in spellCooldowns)
            {
                if (cooldown.CooldownLeft > 0 && !cooldown.OnHold)
                    cooldown.CooldownLeft -= deltaTime;
            }
        }

        public bool IsReady(SpellInfo spellInfo) => !HasCooldown(spellInfo.Id, out _);

        public bool HasCooldown(int spellInfoId, out SpellCooldown cooldown) => spellCooldownsById.TryGetValue(spellInfoId, out cooldown) && cooldown.CooldownLeft > 0;

        public void Handle(SpellCooldownEvent cooldownEvent)
        {
            int expectedCooldownFrames = (int)(cooldownEvent.CooldownTime / BoltNetwork.FrameDeltaTime / 1000.0f);
            int framesPassed = BoltNetwork.ServerFrame - cooldownEvent.ServerFrame;

            if (framesPassed > expectedCooldownFrames || expectedCooldownFrames < 1)
                return;

            float cooldownProgressLeft = 1.0f - (float) framesPassed / expectedCooldownFrames;
            int cooldownTimeLeft = Mathf.RoundToInt(cooldownEvent.CooldownTime * cooldownProgressLeft);

            AddCooldown(cooldownEvent.SpellId, cooldownEvent.CooldownTime, cooldownTimeLeft);
        }

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

        internal void StartCooldown(SpellInfo spellInfo)
        {
            if (spellInfo.IsPassive())
                return;

            int cooldownLeft = spellInfo.CooldownTime;
            if (cooldownLeft <= 0)
                return;

            SpellCooldown spellCooldown = AddCooldown(spellInfo.Id, cooldownLeft, cooldownLeft);

            if (unit is Player player && player.BoltEntity.Controller != null)
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCooldown, player, spellCooldown);
        }

        private SpellCooldown AddCooldown(int spellId, int cooldownTime, int cooldownTimeLeft)
        {
            if (spellCooldownsById.TryGetValue(spellId, out SpellCooldown spellCooldown))
            {
                spellCooldown.Cooldown = cooldownTime;
                spellCooldown.CooldownLeft = cooldownTimeLeft;
                spellCooldown.OnHold = false;
                return spellCooldown;
            }
            else
            {
                var newCooldown = new SpellCooldown(cooldownTime, cooldownTimeLeft, spellId);
                spellCooldownsById[spellId] = newCooldown;
                spellCooldowns.Add(newCooldown);
                return newCooldown;
            }
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