using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public enum NetworkingType
    {
        [PacketHandlerSessionType(typeof(PhotonWorldSession))]
        PhotonUnityNetworking,
    }

    [Serializable]
    public class MultiplayerFrameworkSelection
    {
        [SerializeField, UsedImplicitly] private Transform managerHolder;
        [SerializeField, UsedImplicitly] private NetworkingType networkingType;

        public Transform ManagerHolder => managerHolder;
        public NetworkingType NetworkingType => networkingType;
    }
}
