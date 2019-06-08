using UnityEngine;

namespace Core
{
    public class SpellCastTargets
    {
        private WorldEntity target;
        private WorldEntity source;
        private WorldEntity destination;

        private ulong origEntityTargetId;
        private ulong entityTargetId;

        public SpellCastTargetFlags TargetMask { get; set; }
        public float Speed { get; set; }
        public float Pitch { get; set; }

        public ulong OrigUnitTargetId => origEntityTargetId;
        public ulong EntityTargetId => entityTargetId;

        public WorldEntity Destination => destination;
        public WorldEntity Source => source;
        public WorldEntity Target => target;

        public bool HasSource => TargetMask.HasTargetFlag(SpellCastTargetFlags.SourceLocation);
        public bool HasDest => TargetMask.HasTargetFlag(SpellCastTargetFlags.DestLocation);
        public bool HasTrajectory => !Mathf.Approximately(Speed, 0);
        public float SpeedXY => Speed * Mathf.Cos(Pitch);
        public float SpeedZ => Speed * Mathf.Sin(Pitch);
        public float Distance2D => Source.DistanceTo(Destination);

        public Unit OrigUnitTarget
        {
            set
            {
                if (value == null)
                    return;

                origEntityTargetId = value.NetworkId;
            }
        }

        public Unit UnitTarget
        {
            get => target as Unit;
            set
            {
                if (value == null)
                    return;

                target = value;
                entityTargetId = value.NetworkId;
                TargetMask |= SpellCastTargetFlags.Unit;
            }
        }

        public GameEntity GameEntityTarget
        {
            get => target as GameEntity;
            set
            {
                if (value == null)
                    return;

                target = value;
                entityTargetId = value.NetworkId;
                TargetMask |= SpellCastTargetFlags.GameEntity;
            }
        }

        public SpellCastTargets()
        {
            TargetMask = 0;
            Pitch = 0.0f;
            Speed = 0.0f;
            target = null;
        }

        public SpellCastTargets(Unit caster/*, ref SpellCastRequest spellCastRequest*/)
        {
            /*TargetMask = spellCastRequest.Target.Flags;
            Pitch = 0.0f;
            Speed = 0.0f;

            target = null;
            entityTargetId = spellCastRequest.Target.Unit;

            if (spellCastRequest.Target.SrcLocation != null)
            {
                src.TransportId = spellCastRequest.Target.SrcLocation.Transport;
                Position pos = src.TransportId != NetworkId.Empty ? src.TransportOffset : src.Position;

                pos.Relocate(spellCastRequest.Target.SrcLocation.Location);
                if (spellCastRequest.Target.Orientation != null)
                    pos.SetOrientation(spellCastRequest.Target.Orientation.Value);
            }

            if (spellCastRequest.Target.DstLocation != null)
            {
                dst.TransportId = spellCastRequest.Target.DstLocation.Transport;
                Position pos = dst.TransportId != NetworkId.Empty ? dst.TransportOffset : dst.Position;

                pos.Relocate(spellCastRequest.Target.DstLocation.Location);
                if (spellCastRequest.Target.Orientation != null)
                    pos.SetOrientation(spellCastRequest.Target.Orientation.Value);
            }

            Pitch = spellCastRequest.MissileTrajectory.Pitch;
            Speed = spellCastRequest.MissileTrajectory.Speed;*/

            DoUpdate(caster);
        }

        public void RemoveEntityTarget()
        {
            target = null;
            entityTargetId = 0;
            TargetMask &= ~SpellCastTargetFlags.UnitMask;
        }

        public void SetSrc(WorldEntity entity)
        {
            source = entity;
            TargetMask |= SpellCastTargetFlags.SourceLocation;
        }

        public void RemoveSrc()
        {
            TargetMask &= ~SpellCastTargetFlags.SourceLocation;
        }

        public void SetDst(WorldEntity entity)
        {
            destination = entity;
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void RemoveDst()
        {
            TargetMask &= ~SpellCastTargetFlags.DestLocation;
        }

        public void DoUpdate(Unit caster)
        {
            target = entityTargetId != 0 ? (entityTargetId == caster.NetworkId ? caster : caster.WorldManager.UnitManager.Find(entityTargetId)) : null;

            /*if (HasSource && source.TransportId != 0)
            {
                WorldEntity transport = caster.WorldManager.UnitManager.Find(source.TransportId);
                if (transport != null)
                {
                    //source.Relocate(transport.Position);
                    //source.RelocateOffset(src.TransportOffset);
                }
            }

            if (HasDest && destination.TransportId != 0)
            {
                WorldEntity transport = caster.WorldManager.UnitManager.Find(destination.TransportId);
                if (transport != null)
                {
                    //destination.Relocate(transport.Position);
                    //destination.RelocateOffset(destination.TransportOffset);
                }
            }*/
        }
    }
}