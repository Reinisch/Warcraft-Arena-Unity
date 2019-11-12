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
    internal class WorkflowStandard : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private InterfaceReference interfaceReference;

        private WorldManager worldManager;

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.RegisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
            EventHandler.RegisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(true));
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.UnregisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
            EventHandler.UnregisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
        }

        private void OnGameMapLoaded(string map, NetworkingMode mode)
        {
            bool hasServerLogic = mode == NetworkingMode.Server || mode == NetworkingMode.Both;
            bool hasClientLogic = mode == NetworkingMode.Client || mode == NetworkingMode.Both;

            worldManager = hasServerLogic ? (WorldManager)new WorldServerManager(hasClientLogic) : new WorldClientManager(false);
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, worldManager);

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
            worldManager?.Dispose();
            worldManager = null;

            interfaceReference.HideScreen<BattleScreen>();
            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(false, disconnectReason));
        }
    }
}
