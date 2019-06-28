using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Action Error Display Settings", menuName = "Game Data/Interface/Action Error Display Settings", order = 2)]
    public class ActionErrorDisplaySettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private float lifeTime = 3.0f;
        [SerializeField, UsedImplicitly] private float floatingSpeed = 3;
        [SerializeField, UsedImplicitly] private int fontSize = 20;
        [SerializeField, UsedImplicitly] private bool allowRepeating;
        [SerializeField, UsedImplicitly] private AnimationCurve sizeOverTime;
        [SerializeField, UsedImplicitly] private AnimationCurve alphaOverTime;

        public float LifeTime => lifeTime;
        public float FloatingSpeed => floatingSpeed;
        public int FontSize => fontSize;
        public bool AllowRepeating => allowRepeating;
        public AnimationCurve SizeOverTime => sizeOverTime;
        public AnimationCurve AlphaOverTime => alphaOverTime;
    }
}
