using Common;
using Core;
using Client;
using JetBrains.Annotations;
using Server;
using UdpKit;
using UnityEngine;

namespace Game.Launcher.Standard
{
    [CreateAssetMenu(fileName = "Workflow Standard Reference", menuName = "Game Data/Scriptable/Workflow Standard", order = 1)]
    internal class WorkflowStandard : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private InterfaceReference interfaceReference;

        private WorldManager worldManager;

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<string, GameManager.NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.RegisterEvent<DisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectHandled, OnDisconnectHandled);
            EventHandler.RegisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
            EventHandler.RegisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(true));
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<DisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectHandled, OnDisconnectHandled);
            EventHandler.UnregisterEvent<string, GameManager.NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnGameMapLoaded);
            EventHandler.UnregisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);
            EventHandler.UnregisterEvent<UdpConnectionDisconnectReason>(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, OnDisconnectedFromHost);
        }

        private void OnGameMapLoaded(string map, GameManager.NetworkingMode mode)
        {
            bool hasServerLogic = mode == GameManager.NetworkingMode.Server || mode == GameManager.NetworkingMode.Both;
            bool hasClientLogic = mode == GameManager.NetworkingMode.Client || mode == GameManager.NetworkingMode.Both;

            worldManager = hasServerLogic ? (WorldManager)new WorldServerManager(hasClientLogic) : new WorldClientManager(false);
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, worldManager);

            interfaceReference.HideScreen<LobbyScreen>();
            interfaceReference.ShowScreen<BattleScreen, BattleHudPanel>();
        }

        private void OnDisconnectHandled(DisconnectReason reason)
        {
            interfaceReference.HideScreen<BattleScreen>();
            interfaceReference.ShowScreen<LobbyScreen, LobbyPanel, LobbyPanel.ShowToken>(new LobbyPanel.ShowToken(false, reason));
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

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectHandled, disconnectReason);
        }
    }
}
