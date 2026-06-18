namespace Core
{
    public abstract partial class Unit
    {
        private static NetAuraSlot ToNetAuraSlot(AuraApplication application) => application == null
            ? default
            : new NetAuraSlot
            {
                AuraId = application.Aura.AuraInfo.Id,
                DurationMax = application.Aura.MaxDuration,
                DurationLeft = application.Aura.Duration,
                Charges = application.Aura.Charges,
            };
            
        private NetAuraSlot[] networkAuras;

        public virtual UnitSnapshot CaptureState() => new UnitSnapshot
        {
            Kind = UnitSnapshotKind.Creature,
            Position = Position,
            Rotation = Rotation,
            ModelId = ModelId,
            OriginalModelId = OriginalModelId,
            ClassType = ClassType,
            FactionId = Faction != null ? Faction.FactionId : 0,
            DeathState = DeathState,
            EmoteType = EmoteType,
            VisualEffectFlags = VisualEffects,
            DisplayPowerType = DisplayPowerType,
            DisplayPower = Power,
            DisplayPowerMax = MaxPower,
            Scale = Scale,
            FreeForAll = FreeForAll,
            OriginalAIInfoId = CreationToken != null ? CreationToken.OriginalAIInfoId : 0,
            CreatureInfoId = 0,
            Name = Name,
        };

        /// <summary>
        /// Puts this unit into network-puppet mode (true) or local-authority mode (false). (Polymorph/Root)
        /// </summary>
        public void SetNetworkControlled(bool networkControlled)
        {
            characterController.SetSimulated(!networkControlled);
        }

        public void SetNetworkTarget(Unit target)
        {
            if (Target != target)
                Attributes.UpdateTarget(newTarget: target);
        }

        public void SetNetworkFaction(int factionId, bool freeForAll)
        {
            bool freeForAllChanged = FreeForAll != freeForAll;
            FreeForAll = freeForAll;

            bool factionChanged = false;
            if ((Faction == null || Faction.FactionId != factionId)
                && Balance.FactionsById.TryGetValue(factionId, out FactionDefinition faction))
            {
                Faction = faction; // setter fires UnitFactionChanged
                factionChanged = true;
            }

            if (!factionChanged && freeForAllChanged)
                EventBus.ExecuteEvent(this, Common.GameEvents.UnitFactionChanged);
        }

        public void ApplyNetworkSpeed(UnitMoveType moveType, float rate) => Attributes.UpdateSpeedRate(moveType, rate);

        public void ApplyNetworkTeleport(UnityEngine.Vector3 position) => Teleport(position, notify: false);

        public void SetNetworkCast(SpellInfo spellInfo, int castTime) => Spells.Cast.SetNetworkCast(spellInfo, castTime);

        public void ClearNetworkCast() => Spells.Cast.ClearNetworkCast();

        public void StartCooldownDisplay(SpellInfo spellInfo)
        {
            SpellHistory.StartGlobalCooldown(spellInfo);
            SpellHistory.HandleCooldown(spellInfo);
        }

        /// <summary>
        /// Applies replicated movement to a puppet unit.
        /// </summary>
        public void SetNetworkTransform(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, MovementFlags movementFlags)
        {
            Position = position;
            Rotation = rotation;
            Motion.SetNetworkMovementFlags(movementFlags);
        }

        public UnitVitals CaptureVitals() => new UnitVitals
        {
            Health = Health,
            MaxHealth = MaxHealth,
            Power = Power,
            DeathState = DeathState,
            EmoteType = EmoteType,
            ModelId = ModelId,
            ClassType = ClassType,
            DisplayPowerType = DisplayPowerType,
            ComboPoints = ComboPoints,
            VisualEffects = VisualEffects,
        };

        public void CaptureAuras(NetAuraSlot[] buffer)
        {
            AuraApplication[] applications = VisibleAuras.ApplicationSlots;
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = applications != null && i < applications.Length ? ToNetAuraSlot(applications[i]) : default;
        }

        public void ApplyNetworkAuras(NetAuraSlot[] auras)
        {
            networkAuras = auras;
            VisibleAuras.NotifyChanged();
        }

        public NetAuraSlot GetVisibleAuraSlot(int index)
        {
            if (networkAuras != null)
                return index >= 0 && index < networkAuras.Length ? networkAuras[index] : default;

            AuraApplication[] applications = VisibleAuras.ApplicationSlots;
            return applications != null && index >= 0 && index < applications.Length
                ? ToNetAuraSlot(applications[index])
                : default;
        }

        public void ApplyVitals(in UnitVitals vitals)
        {
            // Max health first: SetHealth clamps to it, so a raised cap must be in place before the new health.
            if (MaxHealth != vitals.MaxHealth)
                Attributes.SetMaxHealth(vitals.MaxHealth);

            if (Health != vitals.Health)
                Attributes.SetHealth(vitals.Health);

            if (ClassType != vitals.ClassType)
                ApplyNetworkClass(vitals.ClassType);

            if (DisplayPowerType != vitals.DisplayPowerType)
            {
                DisplayPowerType = vitals.DisplayPowerType;
                Attributes.SetMaxPower(DisplayPowerType, Attributes.MaxPower(DisplayPowerType));
                Attributes.SetPower(DisplayPowerType, vitals.Power);
            }
            else if (Power != vitals.Power)
            {
                Attributes.SetPower(DisplayPowerType, vitals.Power);
            }

            if (ComboPoints != vitals.ComboPoints)
                Attributes.SetComboPoints(vitals.ComboPoints);

            if (EmoteType != vitals.EmoteType)
                ModifyEmoteState(vitals.EmoteType);

            if (DeathState != vitals.DeathState)
                ModifyDeathState(vitals.DeathState);

            if (ModelId != vitals.ModelId)
                ModelId = vitals.ModelId;

            if (VisualEffects != vitals.VisualEffects)
                Attributes.VisualEffectFlags = vitals.VisualEffects;
        }

        protected virtual void ApplyNetworkClass(ClassType classType) => ClassType = classType;
    }
}
