using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleScreen : UIScreen<BattleScreen>
    {
        [field: SerializeField, UsedImplicitly]
        public RectTransform SpellOverlayRoot { get; private set; }
    }
}
