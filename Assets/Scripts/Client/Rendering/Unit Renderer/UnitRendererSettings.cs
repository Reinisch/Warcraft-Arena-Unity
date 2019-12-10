using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Renderer Settings", menuName = "Game Data/Rendering/Unit Renderer Settings", order = 1)]
    public class UnitRendererSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private float stealthTransparencyAlpha = 0.5f;
        [SerializeField, UsedImplicitly] private float transparencyTransitionSpeed = 1.0f;
        [SerializeField, UsedImplicitly] private float renderInterpolationSmoothTime = 0.05f;

        public float StealthTransparencyAlpha => stealthTransparencyAlpha;
        public float TransparencyTransitionSpeed => transparencyTransitionSpeed;
        public float RenderInterpolationSmoothTime => renderInterpolationSmoothTime;
    }
}