using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class SpellCastTargets
    {
        // entities (can be used at spell creating and after Update at casting)
        private WorldEntity entityTarget;

        // entity Guid/etc, can be used always
        private Guid origEntityTargetGuid;
        private Guid entityTargetGuid;

        private SpellDestination src;
        private SpellDestination dst;

        public SpellCastTargetFlags TargetMask { get; set; }
        public float Speed { get; set; }
        public float Pitch { get; set; }

        public SpellDestination Source => src;
        public Position SourcePos => src.Position;
        public SpellDestination Dest => dst;
        public Position DestPos => dst.Position;

        public Guid OrigUnitTargetGuid => origEntityTargetGuid;
        public Guid EntityTargetGuid => entityTargetGuid;

        public bool HasSource => TargetMask.HasFlag(SpellCastTargetFlags.SourceLocation);
        public bool HasDest => TargetMask.HasFlag(SpellCastTargetFlags.DestLocation);
        public bool HasTrajectory => !Mathf.Approximately(Speed, 0);
        public float Distance2D => src.Position.GetExactDist2D(dst.Position);
        public float SpeedXY => Speed * Mathf.Cos(Pitch);
        public float SpeedZ => Speed * Mathf.Sin(Pitch);

        public Unit OrigUnitTarget
        {
            set
            {
                if (value == null)
                    return;

                origEntityTargetGuid = value.Guid;
            }
        }

        public Unit UnitTarget
        {
            get { return entityTarget as Unit; }
            set
            {
                if (value == null)
                    return;

                entityTarget = value;
                entityTargetGuid = value.Guid;
                TargetMask |= SpellCastTargetFlags.Unit;
            }
        }

        public Corpse CorpseTarget => entityTarget as Corpse;

        public WorldEntity EntityTarget => entityTarget;

        public GameEntity GameEntityTarget
        {
            get { return entityTarget.AsGameEntity; }
            set
            {
                if (value == null)
                    return;

                entityTarget = value;
                entityTargetGuid = value.Guid;
                TargetMask |= SpellCastTargetFlags.GameEntity;
            }
        }
    

        public SpellCastTargets()
        {
            TargetMask = 0;
            Pitch = 0.0f;
            Speed = 0.0f;
            entityTarget = null;
        }

        public SpellCastTargets(Unit caster/*, ref SpellCastRequest spellCastRequest*/)
        {
            /*TargetMask = spellCastRequest.Target.Flags;
            Pitch = 0.0f;
            Speed = 0.0f;

            entityTarget = null;
            entityTargetGuid = spellCastRequest.Target.Unit;

            if (spellCastRequest.Target.SrcLocation != null)
            {
                src.TransportGUID = spellCastRequest.Target.SrcLocation.Transport;
                Position pos = src.TransportGUID != Guid.Empty ? src.TransportOffset : src.Position;

                pos.Relocate(spellCastRequest.Target.SrcLocation.Location);
                if (spellCastRequest.Target.Orientation != null)
                    pos.SetOrientation(spellCastRequest.Target.Orientation.Value);
            }

            if (spellCastRequest.Target.DstLocation != null)
            {
                dst.TransportGUID = spellCastRequest.Target.DstLocation.Transport;
                Position pos = dst.TransportGUID != Guid.Empty ? dst.TransportOffset : dst.Position;

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
            entityTarget = null;
            entityTargetGuid = Guid.Empty;
            TargetMask &= ~SpellCastTargetFlags.UnitMask;
        }

        public void SetSrc(float x, float y, float z)
        {
            src = new SpellDestination(x, y, z);
            TargetMask |= SpellCastTargetFlags.SourceLocation;
        }

        public void SetSrc(Position pos)
        {
            src = new SpellDestination(pos);
            TargetMask |= SpellCastTargetFlags.SourceLocation;
        }

        public void SetSrc(WorldEntity entity)
        {
            src = new SpellDestination(entity);
            TargetMask |= SpellCastTargetFlags.SourceLocation;
        }

        public void ModSrc(Position pos)
        {
            Assert.IsTrue(TargetMask.HasFlag(SpellCastTargetFlags.SourceLocation));
            src.Relocate(pos);
        }

        public void RemoveSrc()
        {
            TargetMask &= ~SpellCastTargetFlags.SourceLocation;
        }

        public void SetDst(float x, float y, float z, float orientation, int mapId = GridHelper.MapIdInvalid)
        {
            dst = new SpellDestination(x, y, z, orientation, mapId);
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void SetDst(Position pos)
        {
            dst = new SpellDestination(pos);
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void SetDst(WorldEntity entity)
        {
            dst = new SpellDestination(entity);
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void SetDst(SpellDestination spellDest)
        {
            dst = spellDest;
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void SetDst(SpellCastTargets spellTargets)
        {
            dst = spellTargets.Dest;
            TargetMask |= SpellCastTargetFlags.DestLocation;
        }

        public void ModDst(Position pos)
        {
            Assert.IsTrue(TargetMask.HasFlag(SpellCastTargetFlags.DestLocation));
            dst.Relocate(pos);
        }

        public void ModDst(SpellDestination spellDest)
        {
            Assert.IsTrue(TargetMask.HasFlag(SpellCastTargetFlags.DestLocation));
            dst = spellDest;
        }

        public void RemoveDst()
        {
            TargetMask &= ~SpellCastTargetFlags.DestLocation;
        }

        public void DoUpdate(Unit caster)
        {
            entityTarget = entityTargetGuid != Guid.Empty ? (entityTargetGuid == caster.Guid ? caster : EntityAccessor.FindWorldEntityOnSameMap(caster, entityTargetGuid)) : null;

            // update positions by transport move
            if (HasSource && src.TransportGUID != Guid.Empty)
            {
                WorldEntity transport = EntityAccessor.FindWorldEntityOnSameMap(caster, src.TransportGUID);
                if (transport != null)
                {
                    src.Position.Relocate(transport.WorldLocation);
                    src.Position.RelocateOffset(src.TransportOffset);
                }
            }

            if (HasDest && dst.TransportGUID != Guid.Empty)
            {
                WorldEntity transport = EntityAccessor.FindWorldEntityOnSameMap(caster, dst.TransportGUID);
                if (transport != null)
                {
                    dst.Position.Relocate(transport.WorldLocation);
                    dst.Position.RelocateOffset(dst.TransportOffset);
                }
            }
        }
    }
}