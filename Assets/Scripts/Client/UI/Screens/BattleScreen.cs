using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleScreen : UIWindowController<BattleScreen>
    {
        [SerializeField, UsedImplicitly] private TransformBattleTagDictionary tagsByKeys;
        [SerializeField, UsedImplicitly] private BattleHudPanel battleHudPanel;

        public new void Initialize(ScreenController controller)
        {
            base.Initialize(controller);

            gameObject.SetActive(false);

            RegisterPanel<BattleHudPanel, BattleHudPanel.RegisterToken>(battleHudPanel);
        }       

        public new void Deinitialize(ScreenController controller)
        {
            UnregisterPanel<BattleHudPanel, BattleHudPanel.UnregisterToken>(battleHudPanel);

            gameObject.SetActive(false);

            base.Deinitialize(controller);
        }

        public RectTransform FindTag(BattleHudTagType tagType) => tagsByKeys.Value(tagType);
    }
}
