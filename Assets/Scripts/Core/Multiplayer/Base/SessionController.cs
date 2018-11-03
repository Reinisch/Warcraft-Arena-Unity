using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class SessionController<TOpCodes>
    {
        private readonly Dictionary<TOpCodes, IPacketHandler<TOpCodes>> packetHandlers = new Dictionary<TOpCodes, IPacketHandler<TOpCodes>>();

        public void Initialize(NetworkingType networkingType)
        {
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                var containerAttribute = type.GetCustomAttributes(typeof(PacketHandlerContainerAttribute), false).SingleOrDefault();
                if (containerAttribute == null || ((PacketHandlerContainerAttribute)containerAttribute).FrameworkType != networkingType)
                    continue;

                var frameWorkTypeInfo = typeof(NetworkingType).GetMember(networkingType.ToString());
                var attributes = frameWorkTypeInfo[0].GetCustomAttributes(typeof(PacketHandlerSessionTypeAttribute), false);
                var sessionType = ((PacketHandlerSessionTypeAttribute)attributes[0]).SessionType;

                foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var attribute = (PacketHandlerAttribute)method.
                        GetCustomAttributes(typeof(PacketHandlerAttribute), false).SingleOrDefault();

                    if (attribute == null)
                        continue;

                    var packet = (IPacket<TOpCodes>)Activator.CreateInstance(method.GetParameters()[1].ParameterType);
                    var del = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(sessionType, packet.GetType()), method);
                    var handler = (IPacketHandler<TOpCodes>)Activator.CreateInstance(typeof(PacketHandler<,,>).
                        MakeGenericType(sessionType, typeof(TOpCodes), packet.GetType()), del);

                    packetHandlers.Add(packet.OpCode, handler);
                }
            }

#if UNITY_EDITOR
            Assert.raiseExceptions = true;
            foreach (TOpCodes opCode in Enum.GetValues(typeof(TOpCodes)))
                Assert.IsTrue(packetHandlers.ContainsKey(opCode), $"Packet handler for OpCode: {opCode} is not defined!");
#endif
        }

        public void Deinitialize()
        {
            packetHandlers.Clear();
        }

        public void HandlePacket(WorldSession session, IPacket<TOpCodes> packet)
        {
            packetHandlers[packet.OpCode].Invoke(session, packet);
        }
    }
}