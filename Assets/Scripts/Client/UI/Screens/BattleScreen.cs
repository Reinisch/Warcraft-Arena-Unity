using Client.UI;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleScreen : UIWindowController<BattlePanelType>
    {
        [SerializeField, UsedImplicitly] private BattleHudPanel battleHudPanel;
        
        private WorldManager worldManager;
        private PhotonBoltClientListener clientListener;

        public void Initialize(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
        {
            gameObject.SetActive(false);

            RegisterPanel(battleHudPanel, new BattleHudPanel.InitData(photonManager, clientListener));
        }       

        public void Deinitialize()
        {
            UnregisterPanel(battleHudPanel, new BattleHudPanel.DefaultDeinitData());

            gameObject.SetActive(false);
        }

        public void InitializeWorld(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public void DeinitializeWorld()
        {
            worldManager = null;
        }
    }
}
