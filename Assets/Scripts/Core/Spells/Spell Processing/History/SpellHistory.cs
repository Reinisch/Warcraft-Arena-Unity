using System.Collections.Generic;
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
        public float GlobalCooldownRatio => (float) GlobalCooldownLeft / GlobalCooldown;
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

            if (cooldownLeft < 0)
                cooldownLeft = 0;

            if (spellCooldownsById.TryGetValue(spellInfo.Id, out SpellCooldown spellCooldown))
            {
                spellCooldown.Cooldown = cooldownLeft;
                spellCooldown.CooldownLeft = cooldownLeft;
                spellCooldown.OnHold = false;
            }
            else
            {
                var newCooldown = new SpellCooldown(cooldownLeft, cooldownLeft);
                spellCooldownsById[spellInfo.Id] = newCooldown;
                spellCooldowns.Add(newCooldown);
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