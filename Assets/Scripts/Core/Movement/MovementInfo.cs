using System;

namespace Core
{
    public class MovementInfo
    {
        public class TransportInfo
        {
            public Guid Guid { get; private set; }
            public Position Pos { get; private set; }
            public byte Seat { get; private set; }
            public uint Time { get; private set; }
            public uint PrevTime { get; private set; }
            public uint VehicleId { get; private set; }

            public TransportInfo()
            {
                Pos = new Position();
                Reset();
            }

            public void Reset()
            {
                Guid = Guid.Empty;
                Pos.Relocate(0.0f, 0.0f, 0.0f, 0.0f);

                Seat = 0;
                Time = 0;
                PrevTime = 0;
                VehicleId = 0;
            }
        }

        public class JumpInfo
        {
            public long FallTime { get; set; }

            public float SpeedY { get; set; }
            public float SpeedXZ { get; set; }
            public float SinAngle { get; private set; }
            public float CosAngle { get; private set; }

            public JumpInfo()
            {
                Reset();
            }

            public void Reset()
            {
                FallTime = 0;
                SpeedY = SpeedXZ = SinAngle = CosAngle = 0.0f;
            }
        }

        public Guid Guid { get; private set; }
        public MovementFlags Flags { get; set; }
        public MovementExtraFlags ExtraFlags { get; private set; }
        public uint Time { get; private set; }
        public float Pitch { get; set; }
        public float SplineElevation { get; private set; }

        public TransportInfo Transport { get; private set; }
        public JumpInfo Jump { get; private set; }
        public Position Pos { get; private set; }


        public MovementInfo()
        {
            Guid = Guid.Empty;

            Flags = 0;
            ExtraFlags = 0;
            Time = 0;
            Pitch = 0.0f;
            SplineElevation = 0.0f;

            Pos = new Position();
            Transport = new TransportInfo();
            Jump = new JumpInfo();
        }

        public void AddMovementFlag(MovementFlags flag) { Flags |= flag; }

        public void RemoveMovementFlag(MovementFlags flag) { Flags &= ~flag; }

        public bool HasMovementFlag(MovementFlags flag) { return (Flags & flag) != 0; }

        public MovementExtraFlags GetExtraMovementFlags() { return ExtraFlags; }
        public void SetExtraMovementFlags(MovementExtraFlags flag) { ExtraFlags = flag; }
        public void AddExtraMovementFlag(MovementExtraFlags flag) { ExtraFlags |= flag; }
        public void RemoveExtraMovementFlag(MovementExtraFlags flag) { ExtraFlags &= ~flag; }
        public bool HasExtraMovementFlag(MovementExtraFlags flag) { return (ExtraFlags & flag) != 0; }

        public long GetFallTime() { return Jump.FallTime; }
        public void SetFallTime(long fallTime) { Jump.FallTime = fallTime; }

        public void ResetTransport()
        {
            Transport.Reset();
        }
        public void ResetJump()
        {
            Jump.Reset();
        }

        protected void OutDebug() { }
    }
}