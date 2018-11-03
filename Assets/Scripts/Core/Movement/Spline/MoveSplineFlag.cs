using System;

namespace Core
{
    [Flags]
    public enum MoveSplineFlags
    {
        None = 0x00000000,
        // x00-x07 used as animation Ids storage in pair with Animation flag
        FallingSlow = 0x00000010,
        Done = 0x00000020,
        Falling = 0x00000040,               // Affects elevation computation, can't be combined with Parabolic flag
        Flying = 0x00000200,                // Smooth movement(Catmullrom interpolation mode), flying animation
        OrientationFixed = 0x00000400,      // Model orientation fixed
        Catmullrom = 0x00000800,            // Used Catmullrom interpolation mode
        Cyclic = 0x00001000,                // Movement by cycled spline
        TransportEnter = 0x00008000,
        TransportExit = 0x00010000,
        OrientationInversed = 0x00080000,
        SmoothGroundPath = 0x00100000,
        Walkmode = 0x00200000,
        UncompressedPath = 0x00400000,
        Animation = 0x02000000,             // Plays animation after some time passed
        Parabolic = 0x04000000,             // Affects elevation computation, can't be combined with Falling flag

        // Masks, animation ids stored here, see AnimType enum, used with Animation flag
        MaskAnimations = 0x7,
        // flags that shouldn't be appended into SMSG_MONSTER_MOVE\SMSG_MONSTER_MOVE_TRANSPORT packet, should be more probably
        MaskNoMonsterMove = MaskAnimations | Done,
    }

    public static class MoveSplineFlagExtensions
    {
        public static bool IsSmooth(this MoveSplineFlags baseFlags) { return (baseFlags & MoveSplineFlags.Catmullrom) > 0; }

        public static bool IsLinear(this MoveSplineFlags baseFlags) { return !IsSmooth(baseFlags); }


        public static byte GetAnimationId(this MoveSplineFlags baseFlags) { return (byte)(baseFlags & MoveSplineFlags.MaskAnimations); }

        public static bool HasAllFlags(this MoveSplineFlags baseFlags, MoveSplineFlags flags) { return (baseFlags & flags) == flags; }

        public static bool HasFlag(this MoveSplineFlags baseFlags, MoveSplineFlags flags) { return (baseFlags & flags) != 0; }

        public static MoveSplineFlags SetFlag(this MoveSplineFlags baseFlags, MoveSplineFlags flags) { return baseFlags | flags; }


        public static MoveSplineFlags EnableAnimation(this MoveSplineFlags baseFlags, byte anim)
        {
            return (baseFlags & ~(MoveSplineFlags.MaskAnimations | MoveSplineFlags.Falling | MoveSplineFlags.Parabolic | MoveSplineFlags.FallingSlow))
                   | MoveSplineFlags.Animation | ((MoveSplineFlags)anim & MoveSplineFlags.MaskAnimations);
        }

        public static MoveSplineFlags EnableParabolic(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~(MoveSplineFlags.MaskAnimations | MoveSplineFlags.Falling | MoveSplineFlags.Animation | MoveSplineFlags.FallingSlow)) | MoveSplineFlags.Parabolic;
        }

        public static MoveSplineFlags EnableFlying(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~MoveSplineFlags.Falling) | MoveSplineFlags.Flying;
        }

        public static MoveSplineFlags EnableFalling(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~(MoveSplineFlags.MaskAnimations | MoveSplineFlags.Parabolic | MoveSplineFlags.Animation | MoveSplineFlags.Flying)) | MoveSplineFlags.Falling;
        }

        public static MoveSplineFlags EnableCatmullRom(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~MoveSplineFlags.SmoothGroundPath) | MoveSplineFlags.Catmullrom;
        }

        public static MoveSplineFlags EnableTransportEnter(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~MoveSplineFlags.TransportExit) | MoveSplineFlags.TransportEnter;
        }

        public static MoveSplineFlags EnableTransportExit(this MoveSplineFlags baseFlags)
        {
            return (baseFlags & ~MoveSplineFlags.TransportEnter) | MoveSplineFlags.TransportExit;
        }
    }
}