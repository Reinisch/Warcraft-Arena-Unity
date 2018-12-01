using Client;
using Core;
using Server;
using UnityEngine;
using JetBrains.Annotations;

namespace Game
{
    public class MultiplayerManager : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private PhotonBoltSharedListener boltSharedListener;
        [SerializeField, UsedImplicitly] private PhotonBoltServerListener boltServerListener;
        [SerializeField, UsedImplicitly] private PhotonBoltClientListener boltClientListener;

        public void Initialize()
        {
            boltSharedListener.enabled = true;
        }

        public void Deinitialize()
        {
            boltSharedListener.enabled = false;
        }

        public void DoUpdate(int deltaTime)
        {
        }
    }
}
