using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Floating Text Settings", menuName = "Game Data/Interface/Floating Text Settings", order = 1)]
    public class FloatingTextSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private float lifeTime = 3.0f;
        [SerializeField, UsedImplicitly] private float randomOffset = 0.2f;
        [SerializeField, UsedImplicitly] private float floatingSpeed = 3;
        [SerializeField, UsedImplicitly] private int fontSize = 120;
        [SerializeField, UsedImplicitly] private Color fontColor;
        [SerializeField, UsedImplicitly] private Material fontMaterial;
        [SerializeField, UsedImplicitly] private AnimationCurve sizeOverTime;
        [SerializeField, UsedImplicitly] private AnimationCurve alphaOverTime;
        [SerializeField, UsedImplicitly] private AnimationCurve sizeOverDistanceToCamera;
        [SerializeField, UsedImplicitly] private AnimationCurve randomOffsetOverDistance;

        public float LifeTime => lifeTime;
        public float RandomOffset => randomOffset;
        public float FloatingSpeed => floatingSpeed;
        public int FontSize => fontSize;
        public Color FontColor => fontColor;
        public Material FontMaterial => fontMaterial;
        public AnimationCurve SizeOverTime => sizeOverTime;
        public AnimationCurve AlphaOverTime => alphaOverTime;
        public AnimationCurve SizeOverDistanceToCamera => sizeOverDistanceToCamera;
        public AnimationCurve RandomOffsetOverDistance => randomOffsetOverDistance;
    }
}
