using System;

namespace Core
{
    [Flags]
    public enum PathType
    {
        Blank = 0x00,           // path not built yet
        Normal = 0x01,          // normal path
        Shortcut = 0x02,        // travel through obstacles, terrain, air, etc (old behavior)
        Incomplete = 0x04,      // we have partial path to follow - getting closer to target
        Nopath = 0x08,          // no valid path at all or error in generating one
        NotUsingPath = 0x10,    // used when we are either flying/swiming or on map w/o mmaps
        Short = 0x20,           // path is longer or equal to its limited path length
    }

    public class PathGenerator
    {
        public PathType Type { get; private set; }
    }
}