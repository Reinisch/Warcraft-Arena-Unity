namespace Client
{
    public static class EffectUtils
    {
        public static void ApplyPositioning(this IEffectEntity effectEntity, IEffectPositioner positioner, IEffectPositionerSettings settings)
        {
            positioner.ApplyPositioning(effectEntity, settings);
        }
    }
}
