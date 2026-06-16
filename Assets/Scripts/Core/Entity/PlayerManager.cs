using System;
using Assets.Scripts.Core;
using UnityEngine;
using Zenject;

namespace Core
{
    public class PlayerManager: MonoBehaviour
    {
        [SerializeField]
        private WorldEntityPrefab playerPrefab;

        [Inject]
        private UnitManager unitManager;

        public Player Player { get; private set; }

        public event Action<bool> EventPlayerChanged;

        private void Awake()
        {
            unitManager.EventEntityDetach += OnEntityDetach;
        }

        private void OnDestroy()
        {
            unitManager.EventEntityDetach -= OnEntityDetach;
        }

        internal Player Create(Entity.CreateToken createToken)
        {
            Player = unitManager.Create<Player>(playerPrefab, createToken).GetComponent<Player>();
            EventPlayerChanged?.Invoke(true);

            return Player;
        }

        private void OnEntityDetach(Unit unit)
        {
            if (Player == unit)
            {
                EventPlayerChanged?.Invoke(false);
                Player = null;
            }
        }
    }
}