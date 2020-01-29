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
    internal sealed class WorkflowDedicated : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private DedicatedServerSettings settings;
        [SerializeField, UsedImplicitly] private int maxRestartAttempts = 3;

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private GameManager gameManager;
        private int restartCount;

        protected override void OnRegistered()
        {
            gameManager = FindObjectOfType<GameManager>();
            settings.Apply();

            EventHandler.RegisterEvent<string, NetworkingMode>(GameEvents.GameMapLoaded, OnMapLoaded);
            EventHandler.RegisterEvent(GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            StartServer();
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<string, NetworkingMode>(GameEvents.GameMapLoaded, OnMapLoaded);
            EventHandler.UnregisterEvent(GameEvents.DisconnectedFromMaster, OnDisconnectedFromMaster);

            restartCount = 0;
            tokenSource.Cancel();

            gameManager = null;
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

            gameManager.CreateWorld(new WorldServer(false));
        }

        private void OnDisconnectedFromMaster()
        {
            Debug.LogWarning("Disconnected from master!");

            gameManager.DestroyWorld();

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
