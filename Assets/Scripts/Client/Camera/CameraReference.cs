using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Reference", menuName = "Game Data/Scriptable/Camera", order = 12)]
    public class CameraReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;

        public WarcraftCamera WarcraftCamera { get; private set; }

        protected override void OnRegistered()
        {
            WarcraftCamera = FindObjectOfType<WarcraftCamera>();

            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);

            WarcraftCamera = null;
        }

        private void OnPlayerControlGained(Player player)
        {
            WarcraftCamera.Target = player;
        }

        private void OnPlayerControlLost(Player player)
        {
            WarcraftCamera.Target = null;
        }
    }
}
