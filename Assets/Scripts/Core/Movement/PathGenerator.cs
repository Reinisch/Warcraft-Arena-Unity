using System;

namespace Core
{
    [Flags]
    public enum PathType
    {
        Normal = 1 << 0,
        Shortcut = 1 << 1,
        InComplete = 1 << 2,
        NoPath = 1 << 3,
        NotUsingPath = 1 << 4,
        Short = 1 << 5,
    }

    public class PathGenerator
    {
        public PathType Type { get; private set; }
    }
}