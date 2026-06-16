using Common;
using Core;
using System.Collections.Generic;
using Client;
using Client.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class WorldSession: MonoBehaviour
    {
        [SerializeField]
        private List<ScriptableReference> scriptableCoreModules;

        [SerializeField]
        private List<ScriptableReference> scriptableClientModules;

        [Inject]
        private World world;

        [Inject]
        private InterfaceReference interfaceModule;

        public List<ScriptableReference> CoreModules => scriptableCoreModules;
        public List<ScriptableReference> ClientModules => scriptableClientModules;

        private void Awake()
        {
            for (var i = 0; i < scriptableCoreModules.Count; i++)
                scriptableCoreModules[i].Register();

            for (var i = 0; i < scriptableClientModules.Count; i++)
                scriptableClientModules[i].Register();

            for (var i = 0; i < scriptableClientModules.Count; i++)
                (scriptableClientModules[i] as ScriptableReferenceClient)?.OnWorldStateChanged(true);
        }

        // Boot into a clean, session-less state (no world, no networking): just show the lobby so the player
        // explicitly chooses Single Player / Create Server / Join. Done in Start (not Awake) so the UI screens
        // — instantiated during module registration in Awake — already exist.
        private void Start()
        {
            interfaceModule.ShowScreen<LobbyScreen, LobbyPanel>();
        }

        private void OnApplicationQuit()
        {
            world.Dispose(applicationQuitting: true);

            for (var i = 0; i < scriptableClientModules.Count; i++)
                (scriptableClientModules[i] as ScriptableReferenceClient)?.OnWorldStateChanged(false);

            for (var i = 0; i < scriptableClientModules.Count; i++)
                scriptableClientModules[i].Unregister();

            for (var i = 0; i < scriptableCoreModules.Count; i++)
                scriptableCoreModules[i].Unregister();
        }
    }
}
