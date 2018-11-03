using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core
{
    public class MultiplayerManager : SingletonGameObject<MultiplayerManager>
    {
        [SerializeField] private NetworkingType networkingType = NetworkingType.PhotonUnityNetworking;
        [SerializeField] private List<MultiplayerFrameworkSelection> availableFrameworksInfo;

        private readonly List<ISingletonGameObject> multiplayerManagers = new List<ISingletonGameObject>();

        /// <summary>
        /// All available packets from server(true) or client(false) by opcode.
        /// </summary>
        public static readonly Dictionary<bool, Dictionary<int, Type>> PacketTypes = new Dictionary<bool, Dictionary<int, Type>>();
        public NetworkingType NetworkingType => networkingType;

        public event Action<WorldPacket, PacketTargets> EventSendToTargets;
        public event Action<WorldPacket, WorldSession> EventSendToTarget;

        public event Action<ClientPacket, WorldSession> EventReceivedFromClient;
        public event Action<ServerPacket> EventReceivedFromServer;

        public override void Initialize()
        {
            base.Initialize();

            PacketTypes[true] = new Dictionary<int, Type>();
            PacketTypes[false] = new Dictionary<int, Type>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(WorldPacket)))
                    continue;

                if(type.IsSubclassOf(typeof(ServerPacket)) && GameManager.Instance.HasClientLogic)
                    PacketTypes[true][((WorldPacket)Activator.CreateInstance(type)).OpCodeIndex] = type;
                else if (type.IsSubclassOf(typeof(ClientPacket)) && GameManager.Instance.HasServerLogic)
                    PacketTypes[false][((WorldPacket)Activator.CreateInstance(type)).OpCodeIndex] = type;
            }

            var frameworkSelection = availableFrameworksInfo.Find(selection => selection.NetworkingType == networkingType);
            multiplayerManagers.AddRange(frameworkSelection.ManagerHolder.GetComponents<ISingletonGameObject>());
            multiplayerManagers.ForEach(manager => manager.Initialize());
        }

        public override void Deinitialize()
        {
            multiplayerManagers.ForEach(manager => manager.Deinitialize());
            multiplayerManagers.Clear();

            PacketTypes.Clear();

            base.Deinitialize();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            multiplayerManagers.ForEach(manager => manager.DoUpdate(deltaTime));
        }


        public void PlayerConnected(WorldSession session)
        {
        }

        public void PlayerDisconnected(WorldSession session)
        {
        }

        public void SendToTargets(WorldPacket packet, PacketTargets targets)
        {
            EventSendToTargets?.Invoke(packet, targets);
        }

        public void SendToTarget(WorldPacket packet, WorldSession target)
        {
            EventSendToTarget?.Invoke(packet, target);
        }

        public void ReceivedFromServer(ServerPacket packet)
        {
            EventReceivedFromServer?.Invoke(packet);
        }

        public void ReceivedFromClient(ClientPacket packet, WorldSession session)
        {
            EventReceivedFromClient?.Invoke(packet, session);
        }
    }
}
