using System;
using UnityEngine.Assertions;

namespace Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandlerContainerAttribute : Attribute
    {
        public NetworkingType FrameworkType { get; }

        public PacketHandlerContainerAttribute(NetworkingType frameworkType)
        {
            FrameworkType = frameworkType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PacketHandlerSessionTypeAttribute : Attribute
    {
        public Type SessionType { get; }

        public PacketHandlerSessionTypeAttribute(Type sessionType)
        {
            Assert.IsTrue(sessionType.IsSubclassOf(typeof(WorldSession)), $"Trying to use {sessionType} as WorldSession in multiplayer framework!");
            SessionType = sessionType;
        }
    }
}
