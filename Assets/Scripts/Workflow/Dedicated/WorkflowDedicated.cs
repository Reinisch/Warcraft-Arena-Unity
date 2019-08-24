using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Core;
using JetBrains.Annotations;
using Server;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Game.Workflow.Dedicated
{
    [CreateAssetMenu(fileName = "Workflow Dedicated Reference", menuName = "Game Data/Scriptable/Workflow Dedicated", order = 1)]
    internal class WorkflowDedicated : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private int maxRestartAttempts = 3;
        
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private WorldManager worldManager;
        private int restartCount;

        protected override void OnRegistered()
        {
            Application.targetFrameRate = 60;

            EventHandler.RegisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnMapLoaded);
            EventHandler.RegisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            StartServer();
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<string, NetworkingMode>(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, OnMapLoaded);
            EventHandler.UnregisterEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            restartCount = 0;
            tokenSource.Cancel();
        }

        private void StartServer()
        {
            photon.StartServer(new ServerRoomToken("Dedicated Server", "Server", "Lordaeron"), false, OnSuccess, OnFail);

            void OnSuccess()
            {
                Debug.LogWarning("Server start successful!");
            }

            void OnFail()
            {
                Debug.LogWarning("Server start failed!");

                HandleRestart();
            }
        }

        private void OnMapLoaded(string map, NetworkingMode mode)
        {
            Assert.AreEqual(mode, NetworkingMode.Server);

            worldManager = new WorldServerManager(false);
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, worldManager);
        }

        private void OnDisconnectedFromMaster()
        {
            Debug.LogWarning("Disconnected from master!");

            worldManager?.Dispose();
            worldManager = null;

            HandleRestart();
        }

        private async void HandleRestart()
        {
            if (restartCount >= maxRestartAttempts)
                Application.Quit();
            else
            {
                Console.WriteLine($"Attempting to restart server! Attempts left: {maxRestartAttempts - restartCount}");

                try
                {
                    await Task.Delay(3000, tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                restartCount++;
                StartServer();
            }
        }
    }
}
