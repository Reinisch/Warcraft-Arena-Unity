using Common;
using Core;
using Client;
using JetBrains.Annotations;
using Server;
using UdpKit;
using UnityEngine;

namespace Game.Workflow.Standard
{
    [CreateAssetMenu(fileName = "Workflow Standard Reference", menuName = "Game Data/Scriptable/Workflow Standard", order = 1)]
    internal sealed class WorkflowStandard : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private InterfaceReference interfaceReference;

        private GameManager gameManager;

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<string, NetworkingMode>(GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.RegisterEvent<UdpConnectionDisconnectReason>(GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
            EventHandler.RegisterEvent(GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            gameManager = FindObjectOfType<GameManager>();
            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(true));
        }

        protected override void OnUnregister()
        {
            gameManager = null;

            EventHandler.UnregisterEvent<string, NetworkingMode>(GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.UnregisterEvent(GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
            EventHandler.UnregisterEvent<UdpConnectionDisconnectReason>(GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
        }

        private void OnGameMapLoaded(string map, NetworkingMode mode)
        {
            bool hasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            bool hasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;

            gameManager.CreateWorld(hasServerLogic ? (World)new WorldServer(hasClientLogic) : new WorldClient(false));

            interfaceReference.HideScreen<LobbyScreen>();
            interfaceReference.ShowScreen<BattleScreen, BattleHudPanel>();
        }

        private void OnDisconnectedFromMaster()
        {
            ProcessDisconnect(DisconnectReason.DisconnectedFromMaster);
        }

        private void OnDisconnectedFromHost(UdpConnectionDisconnectReason udpDisconnectReason)
        {
            ProcessDisconnect(udpDisconnectReason.ToDisconnectReason());
        }

        private void ProcessDisconnect(DisconnectReason disconnectReason)
        {
            gameManager.DestroyWorld();

            interfaceReference.HideScreen<BattleScreen>();
            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(false, disconnectReason));
        }
    }
}
