using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleScreen : UIWindowController
    {
        [SerializeField, UsedImplicitly] private BattleHudPanel battleHudPanel;
        
        private PhotonBoltClientListener clientListener;

        public void Initialize(PhotonBoltClientListener clientListener, ScreenController controller)
        {
            Initialize(controller);

            gameObject.SetActive(false);

            RegisterPanel(battleHudPanel, new BattleHudPanel.RegisterToken(clientListener));
        }       

        public new void Deinitialize(ScreenController controller)
        {
            UnregisterPanel(battleHudPanel, new BattleHudPanel.UnregisterToken());

            gameObject.SetActive(false);

            base.Deinitialize(controller);
        }
    }
}
