namespace Core
{
    public abstract partial class Unit
    {
        /// <summary>
        /// Captures this unit's replicable spawn state so the network layer can mirror it and recreate
        /// the unit on clients. Core owns what's replicable; adapters only serialize the result.
        /// Creature/Player override to add kind-specific fields.
        /// </summary>
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
        /// Puts this unit into network-puppet mode (true) or local-authority mode (false). A puppet's local
        /// physics simulation is disabled — its transform is driven purely by replicated state. The net
        /// layer sets this from NetworkObject ownership (non-owner = puppet); when the server takes movement
        /// control (Polymorph/Root) it re-owns the unit, flipping it back to local authority on the server.
        /// </summary>
        public void SetNetworkControlled(bool networkControlled)
        {
            if (characterController != null)
                characterController.SetSimulated(!networkControlled);
        }

        /// <summary>Applies a replicated target on a client (display only — selection circle, target-of-target).
        /// Idempotent so UnitTargetChanged only fires on a real change.</summary>
        public void SetNetworkTarget(Unit target)
        {
            if (Target != target)
                Attributes.UpdateTarget(newTarget: target);
        }

        /// <summary>Client: apply a replicated movement-speed rate so the owned player moves at server speed
        /// (e.g. Blazing Speed). Its movement is client-authoritative, so the rate must be pushed to it.</summary>
        public void ApplyNetworkSpeed(UnitMoveType moveType, float rate) => Attributes.UpdateSpeedRate(moveType, rate);

        /// <summary>Client: apply a server-initiated teleport on the owned player (notify:false so it doesn't
        /// re-fire the server teleport event); the new position then replicates back via the transform channel.</summary>
        public void ApplyNetworkTeleport(UnityEngine.Vector3 position) => Teleport(position, notify: false);

        /// <summary>Client: drive the replicated cast bar for this unit (server is casting spellInfo).</summary>
        public void SetNetworkCast(SpellInfo spellInfo, int castTime) => Spells.Cast.SetNetworkCast(spellInfo, castTime);

        /// <summary>Client: clear the replicated cast bar (cast finished/interrupted server-side).</summary>
        public void ClearNetworkCast() => Spells.Cast.ClearNetworkCast();

        /// <summary>Client display: start GCD + this spell's cooldown/charge locally when a cast succeeds.
        /// Deterministic from SpellInfo (HandleCooldown/StartGlobalCooldown use SpellInfo values, no haste),
        /// so it matches the server; the client's SpellController ticks it down for the action bar.</summary>
        public void StartCooldownDisplay(SpellInfo spellInfo)
        {
            SpellHistory.StartGlobalCooldown(spellInfo);
            SpellHistory.HandleCooldown(spellInfo);
        }

        /// <summary>
        /// Applies replicated movement to a puppet unit (its motor is disabled, so set transform directly).
        /// MovementFlags drive locomotion animation, so they're applied here too.
        /// </summary>
        public void SetNetworkTransform(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, MovementFlags movementFlags)
        {
            Position = position;
            Rotation = rotation;
            Motion.SetNetworkMovementFlags(movementFlags);
        }

        /// <summary>Current server-authoritative vitals for replication.</summary>
        public UnitVitals CaptureVitals() => new UnitVitals
        {
            Health = Health,
            Power = Power,
            DeathState = DeathState,
            EmoteType = EmoteType,
            ModelId = ModelId,
            ClassType = ClassType,
            DisplayPowerType = DisplayPowerType,
            ComboPoints = ComboPoints,
            VisualEffects = VisualEffects,
        };

        /// <summary>Client: apply a replicated class. Base just sets the value (fires UnitClassChanged);
        /// <see cref="Player"/> overrides to run the full class-change (rebuilds powers, and the owning
        /// client's action bar via the event). Server-only spellbook building stays gated off on the client.</summary>
        protected virtual void ApplyNetworkClass(ClassType classType) => ClassType = classType;

        // Replicated visible auras (display only) on a client; null on the authority, which reads its real
        // VisibleAuras instead. Index == visible slot.
        private NetAuraSlot[] networkAuras;

        /// <summary>Server: snapshot the visible aura slots (id/duration/charges) into a reusable buffer
        /// (length <see cref="MaxVisibleAuraSlots"/>) for replication, avoiding a per-tick allocation.</summary>
        public void CaptureAuras(NetAuraSlot[] buffer)
        {
            AuraApplication[] applications = VisibleAuras.ApplicationSlots;
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = applications != null && i < applications.Length ? ToNetAuraSlot(applications[i]) : default;
        }

        /// <summary>Client: store the replicated auras + refresh the visuals.</summary>
        public void ApplyNetworkAuras(NetAuraSlot[] auras)
        {
            networkAuras = auras;
            VisibleAuras.NotifyChanged();
        }

        /// <summary>The visible aura at a slot — replicated on a client, or the real aura on the authority.</summary>
        public NetAuraSlot GetVisibleAuraSlot(int index)
        {
            if (networkAuras != null)
                return index >= 0 && index < networkAuras.Length ? networkAuras[index] : default;

            AuraApplication[] applications = VisibleAuras.ApplicationSlots;
            return applications != null && index >= 0 && index < applications.Length
                ? ToNetAuraSlot(applications[index])
                : default;
        }

        private static NetAuraSlot ToNetAuraSlot(AuraApplication application) => application == null
            ? default
            : new NetAuraSlot
            {
                AuraId = application.Aura.AuraInfo.Id,
                DurationMax = application.Aura.MaxDuration,
                DurationLeft = application.Aura.Duration,
                Charges = application.Aura.Charges,
            };

        /// <summary>Applies replicated vitals on a client (display only). Idempotent — only acts on changes,
        /// so death/emote transitions fire their visuals once.</summary>
        public void ApplyVitals(in UnitVitals vitals)
        {
            if (Health != vitals.Health)
                Attributes.SetHealth(vitals.Health);

            // Class switch FIRST — it rebuilds the class's power slots (UpdateAvailablePowers) so the display
            // power below can target them (e.g. a druid's Energy). Client-safe (spellbook build stays server-
            // gated); rebuilds the owning client's action bar / powers via the class-change event.
            if (ClassType != vitals.ClassType)
                ApplyNetworkClass(vitals.ClassType);

            // Display power TYPE changes at runtime (e.g. Cat Form → Energy). Apply the type, then refresh the
            // bar's max (from the local power definition) + current (replicated) so it fully switches color +
            // fill; otherwise just the current value when only it changed.
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

            // Combo points (gained/consumed by Cat Form abilities). Fires UnitAttributeChanged(ComboPoints).
            if (ComboPoints != vitals.ComboPoints)
                Attributes.SetComboPoints(vitals.ComboPoints);

            if (EmoteType != vitals.EmoteType)
                ModifyEmoteState(vitals.EmoteType);

            if (DeathState != vitals.DeathState)
                ModifyDeathState(vitals.DeathState);

            // Display model swap (e.g. Polymorph → sheep). Setting ModelId fires UnitModelChanged, which the
            // renderer listens to; idempotent so the model only rebuilds on an actual change.
            if (ModelId != vitals.ModelId)
                ModelId = vitals.ModelId;

            // Visual-effect flags (stealth/invisibility transparency). Setting them fires UnitVisualsChanged,
            // which the renderer listens to to fade the model in/out; idempotent so it only transitions once.
            if (VisualEffects != vitals.VisualEffects)
                Attributes.VisualEffectFlags = vitals.VisualEffects;
        }
    }
}
