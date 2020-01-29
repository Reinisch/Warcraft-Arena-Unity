using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public class SpellHistory
    {
        private readonly Dictionary<int, SpellCooldown> spellCooldownsById = new Dictionary<int, SpellCooldown>();
        private readonly Dictionary<int, List<SpellChargeCooldown>> spellChargesById = new Dictionary<int, List<SpellChargeCooldown>>();
        private readonly List<SpellCooldown> spellCooldowns = new List< SpellCooldown>();
        private readonly Unit caster;
        private readonly IUnitState casterState;

        public int GlobalCooldown { get; private set; }
        public int GlobalCooldownLeft { get; private set; }
        public bool HasGlobalCooldown => GlobalCooldownLeft > 0;

        internal SpellHistory(Unit unit, IUnitState unitState)
        {
            caster = unit;
            casterState = unitState;

            if (!caster.IsOwner)
                caster.AddCallback(nameof(IUnitState.GlobalCooldown), OnGlobalCooldownChanged);
        }

        internal void Detached()
        {
            if (!caster.IsOwner)
                caster.RemoveCallback(nameof(IUnitState.GlobalCooldown), OnGlobalCooldownChanged);

            spellCooldowns.Clear();
            spellCooldownsById.Clear();
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

            foreach (var chargeEntry in spellChargesById)
            {
                int remainingDeltaTime = deltaTime;

                while (chargeEntry.Value.Count > 0 && remainingDeltaTime > 0)
                {
                    SpellChargeCooldown currentChargeCooldown = chargeEntry.Value[0];
                    int handledDelta = Mathf.Min(currentChargeCooldown.ChargeTimeLeft, remainingDeltaTime);

                    currentChargeCooldown.ChargeTimeLeft -= handledDelta;
                    remainingDeltaTime -= handledDelta;

                    if (currentChargeCooldown.ChargeTimeLeft == 0)
                        chargeEntry.Value.RemoveAt(0);
                }
            }
        }

        public bool IsReady(SpellInfo spellInfo)
        {
            if (spellInfo.IsUsingCharges)
                return HasCharge(spellInfo, out _, out _);

            return !HasCooldown(spellInfo.Id, out _);
        }

        public bool HasCooldown(int spellInfoId, out SpellCooldown cooldown)
        {
            return spellCooldownsById.TryGetValue(spellInfoId, out cooldown) && cooldown.CooldownLeft > 0;
        } 

        public bool HasCharge(SpellInfo spellInfo, out SpellChargeCooldown chargeCooldown, out int availableCharges)
        {
            chargeCooldown = null;
            availableCharges = spellInfo.Charges;

            if (!spellInfo.IsUsingCharges)
                return true;

            if (!spellChargesById.TryGetValue(spellInfo.Id, out List<SpellChargeCooldown> chargeCooldowns))
                return true;

            chargeCooldown = chargeCooldowns.Count > 0 ? chargeCooldowns[0] : null;
            availableCharges = spellInfo.Charges - chargeCooldowns.Count;

            return availableCharges > 0;
        }

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

        public void Handle(SpellChargeEvent chargeEvent)
        {
            if (spellChargesById.TryGetValue(chargeEvent.SpellId, out List<SpellChargeCooldown> chargeCooldowns))
            {
                if (chargeCooldowns.Count > 0)
                {
                    AddCharge(chargeEvent.SpellId, chargeEvent.CooldownTime, chargeEvent.CooldownTime);
                    return;
                }
            }

            int expectedCooldownFrames = (int)(chargeEvent.CooldownTime / BoltNetwork.FrameDeltaTime / 1000.0f);
            int framesPassed = BoltNetwork.ServerFrame - chargeEvent.ServerFrame;

            if (framesPassed > expectedCooldownFrames || expectedCooldownFrames < 1)
                return;

            float cooldownProgressLeft = 1.0f - (float)framesPassed / expectedCooldownFrames;
            int cooldownTimeLeft = Mathf.RoundToInt(chargeEvent.CooldownTime * cooldownProgressLeft);

            AddCharge(chargeEvent.SpellId, chargeEvent.CooldownTime, cooldownTimeLeft);
        }

        internal void StartGlobalCooldown(SpellInfo spellInfo)
        {
            if (spellInfo.HasAttribute(SpellExtraAttributes.DoesNotTriggerGcd))
                return;

            if (GlobalCooldownLeft > spellInfo.GlobalCooldownTime)
                return;

            GlobalCooldown = spellInfo.GlobalCooldownTime;
            GlobalCooldownLeft = GlobalCooldown;

            casterState.GlobalCooldown.CooldownTime = GlobalCooldown;
            casterState.GlobalCooldown.ServerFrame = BoltNetwork.ServerFrame;
        }

        internal void HandleCooldown(SpellInfo spellInfo)
        {
            if (spellInfo.IsPassive)
                return;

            int cooldownLeft = spellInfo.CooldownTime;
            if (cooldownLeft <= 0)
                return;

            if (spellInfo.IsUsingCharges)
            {
                SpellChargeCooldown spellChargeCooldown = AddCharge(spellInfo.Id, cooldownLeft, cooldownLeft);

                if (caster is Player player && player.BoltEntity.Controller != null)
                    EventHandler.ExecuteEvent(GameEvents.ServerSpellCharge, player, spellChargeCooldown);
            }
            else
            {
                SpellCooldown spellCooldown = AddCooldown(spellInfo.Id, cooldownLeft, cooldownLeft);

                if (caster is Player player && player.BoltEntity.Controller != null)
                    EventHandler.ExecuteEvent(GameEvents.ServerSpellCooldown, player, spellCooldown);
            }
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

            var newCooldown = new SpellCooldown(cooldownTime, cooldownTimeLeft, spellId);
            spellCooldownsById[spellId] = newCooldown;
            spellCooldowns.Add(newCooldown);
            return newCooldown;
        }

        private SpellChargeCooldown AddCharge(int spellId, int chargeTime, int chargeTimeLeft)
        {
            if (!spellChargesById.TryGetValue(spellId, out List<SpellChargeCooldown> spellCharges))
            {
                spellCharges = new List<SpellChargeCooldown>();
                spellChargesById.Add(spellId, spellCharges);
            }

            var newSpellCharge = new SpellChargeCooldown(chargeTime, chargeTimeLeft, spellId);
            spellCharges.Add(newSpellCharge);
            return newSpellCharge;
        }

        private void OnGlobalCooldownChanged()
        {
            if (!caster.IsController)
                return;

            GlobalCooldown = casterState.GlobalCooldown.CooldownTime;

            int expectedGlobalFrames = (int)(GlobalCooldown / BoltNetwork.FrameDeltaTime / 1000.0f);
            if (expectedGlobalFrames > 0)
            {
                int globalServerFrame = casterState.GlobalCooldown.ServerFrame;
                float cooldownLeftRatio = 1 - Mathf.Clamp01((float) (BoltNetwork.ServerFrame - globalServerFrame) / expectedGlobalFrames);

                GlobalCooldownLeft = Mathf.RoundToInt(cooldownLeftRatio * GlobalCooldown);
            }
            else
                GlobalCooldownLeft = 0;
        }
    }
}