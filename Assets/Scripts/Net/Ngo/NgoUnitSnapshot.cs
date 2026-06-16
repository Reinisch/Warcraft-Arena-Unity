using System;
using Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// NGO-serializable mirror of <see cref="Core.UnitSnapshot"/> (enums as int, name as FixedString,
    /// vectors as components) so it can ride in a NetworkVariable. Conversion keeps Core framework-free.
    /// </summary>
    internal struct NgoUnitSnapshot : INetworkSerializable, IEquatable<NgoUnitSnapshot>
    {
        public int Kind;
        public Vector3 Position;
        public Quaternion Rotation;
        public int ModelId;
        public int OriginalModelId;
        public int ClassType;
        public int FactionId;
        public int DeathState;
        public int EmoteType;
        public int VisualEffectFlags;
        public int DisplayPowerType;
        public int DisplayPower;
        public int DisplayPowerMax;
        public float Scale;
        public bool FreeForAll;
        public int OriginalAIInfoId;
        public int CreatureInfoId;
        public FixedString64Bytes Name;

        public static NgoUnitSnapshot From(in UnitSnapshot s) => new NgoUnitSnapshot
        {
            Kind = (int)s.Kind,
            Position = s.Position,
            Rotation = s.Rotation,
            ModelId = s.ModelId,
            OriginalModelId = s.OriginalModelId,
            ClassType = (int)s.ClassType,
            FactionId = s.FactionId,
            DeathState = (int)s.DeathState,
            EmoteType = (int)s.EmoteType,
            VisualEffectFlags = (int)s.VisualEffectFlags,
            DisplayPowerType = (int)s.DisplayPowerType,
            DisplayPower = s.DisplayPower,
            DisplayPowerMax = s.DisplayPowerMax,
            Scale = s.Scale,
            FreeForAll = s.FreeForAll,
            OriginalAIInfoId = s.OriginalAIInfoId,
            CreatureInfoId = s.CreatureInfoId,
            Name = s.Name ?? string.Empty,
        };

        public UnitSnapshot To() => new UnitSnapshot
        {
            Kind = (UnitSnapshotKind)Kind,
            Position = Position,
            Rotation = Rotation,
            ModelId = ModelId,
            OriginalModelId = OriginalModelId,
            ClassType = (ClassType)ClassType,
            FactionId = FactionId,
            DeathState = (DeathState)DeathState,
            EmoteType = (EmoteType)EmoteType,
            VisualEffectFlags = (UnitVisualEffectFlags)VisualEffectFlags,
            DisplayPowerType = (SpellPowerType)DisplayPowerType,
            DisplayPower = DisplayPower,
            DisplayPowerMax = DisplayPowerMax,
            Scale = Scale,
            FreeForAll = FreeForAll,
            OriginalAIInfoId = OriginalAIInfoId,
            CreatureInfoId = CreatureInfoId,
            Name = Name.ToString(),
        };

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Kind);
            serializer.SerializeValue(ref Position.x);
            serializer.SerializeValue(ref Position.y);
            serializer.SerializeValue(ref Position.z);
            serializer.SerializeValue(ref Rotation.x);
            serializer.SerializeValue(ref Rotation.y);
            serializer.SerializeValue(ref Rotation.z);
            serializer.SerializeValue(ref Rotation.w);
            serializer.SerializeValue(ref ModelId);
            serializer.SerializeValue(ref OriginalModelId);
            serializer.SerializeValue(ref ClassType);
            serializer.SerializeValue(ref FactionId);
            serializer.SerializeValue(ref DeathState);
            serializer.SerializeValue(ref EmoteType);
            serializer.SerializeValue(ref VisualEffectFlags);
            serializer.SerializeValue(ref DisplayPowerType);
            serializer.SerializeValue(ref DisplayPower);
            serializer.SerializeValue(ref DisplayPowerMax);
            serializer.SerializeValue(ref Scale);
            serializer.SerializeValue(ref FreeForAll);
            serializer.SerializeValue(ref OriginalAIInfoId);
            serializer.SerializeValue(ref CreatureInfoId);
            serializer.SerializeValue(ref Name);
        }

        public bool Equals(NgoUnitSnapshot other) =>
            Kind == other.Kind && Position == other.Position && Rotation == other.Rotation &&
            ModelId == other.ModelId && OriginalModelId == other.OriginalModelId &&
            ClassType == other.ClassType && FactionId == other.FactionId &&
            DeathState == other.DeathState && EmoteType == other.EmoteType &&
            VisualEffectFlags == other.VisualEffectFlags && DisplayPowerType == other.DisplayPowerType &&
            DisplayPower == other.DisplayPower && DisplayPowerMax == other.DisplayPowerMax &&
            Scale.Equals(other.Scale) && FreeForAll == other.FreeForAll &&
            OriginalAIInfoId == other.OriginalAIInfoId && CreatureInfoId == other.CreatureInfoId &&
            Name.Equals(other.Name);

        public override bool Equals(object obj) => obj is NgoUnitSnapshot other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Kind, ModelId, FactionId, CreatureInfoId, Name);
    }
}
