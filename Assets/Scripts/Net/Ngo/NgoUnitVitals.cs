using System;
using Core;
using Unity.Netcode;

namespace Net.Ngo
{
    /// <summary>
    /// NGO-serializable mirror of <see cref="Core.UnitVitals"/> (health/power/death/emote) for the
    /// server-authoritative per-tick state channel on <see cref="EntityNetworkView"/>.
    /// </summary>
    internal struct NgoUnitVitals : INetworkSerializable, IEquatable<NgoUnitVitals>
    {
        public int Health;
        public int MaxHealth;
        public int Power;
        public int DeathState;
        public int EmoteType;
        public int ModelId;
        public int ClassType;
        public int DisplayPowerType;
        public int ComboPoints;
        public int VisualEffects;

        public static NgoUnitVitals From(in UnitVitals v) => new NgoUnitVitals
        {
            Health = v.Health,
            MaxHealth = v.MaxHealth,
            Power = v.Power,
            DeathState = (int)v.DeathState,
            EmoteType = (int)v.EmoteType,
            ModelId = v.ModelId,
            ClassType = (int)v.ClassType,
            DisplayPowerType = (int)v.DisplayPowerType,
            ComboPoints = v.ComboPoints,
            VisualEffects = (int)v.VisualEffects,
        };

        public UnitVitals To() => new UnitVitals
        {
            Health = Health,
            MaxHealth = MaxHealth,
            Power = Power,
            DeathState = (DeathState)DeathState,
            EmoteType = (EmoteType)EmoteType,
            ModelId = ModelId,
            ClassType = (ClassType)ClassType,
            DisplayPowerType = (SpellPowerType)DisplayPowerType,
            ComboPoints = ComboPoints,
            VisualEffects = (UnitVisualEffectFlags)VisualEffects,
        };

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Health);
            serializer.SerializeValue(ref MaxHealth);
            serializer.SerializeValue(ref Power);
            serializer.SerializeValue(ref DeathState);
            serializer.SerializeValue(ref EmoteType);
            serializer.SerializeValue(ref ModelId);
            serializer.SerializeValue(ref ClassType);
            serializer.SerializeValue(ref DisplayPowerType);
            serializer.SerializeValue(ref ComboPoints);
            serializer.SerializeValue(ref VisualEffects);
        }

        public bool Equals(NgoUnitVitals other) =>
            Health == other.Health && MaxHealth == other.MaxHealth && Power == other.Power &&
            DeathState == other.DeathState && EmoteType == other.EmoteType &&
            ModelId == other.ModelId && ClassType == other.ClassType &&
            DisplayPowerType == other.DisplayPowerType && ComboPoints == other.ComboPoints &&
            VisualEffects == other.VisualEffects;

        public override bool Equals(object obj) => obj is NgoUnitVitals other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Health, MaxHealth, Power, DeathState, EmoteType, ModelId, ClassType,
            HashCode.Combine(DisplayPowerType, ComboPoints, VisualEffects));
    }
}
