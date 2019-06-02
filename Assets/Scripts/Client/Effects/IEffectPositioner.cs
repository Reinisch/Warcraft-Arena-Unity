namespace Client
{
    public interface IEffectPositioner
    {
        void ApplyPositioning(IEffectEntity effectEntity, IEffectPositionerSettings settings);
    }
}
