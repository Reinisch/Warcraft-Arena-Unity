using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Selection Circle Settings", menuName = "Game Data/Rendering/Selection Circle Settings", order = 1)]
    public class SelectionCircleSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private Color friendlyColor;
        [SerializeField, UsedImplicitly] private Color neutralColor;
        [SerializeField, UsedImplicitly] private Color enemyColor;
        [SerializeField, UsedImplicitly] private EffectTagType targetTag;

        public Color FriendlyColor => friendlyColor;
        public Color NeutralColor => neutralColor;
        public Color EnemyColor => enemyColor;
        public EffectTagType TargetTag => targetTag;
    }
}