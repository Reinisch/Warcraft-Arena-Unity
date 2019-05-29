namespace Client
{
    public interface IEffectPositionerSettings
    {
        bool AttachToTag { get; }

        EffectTagType EffectTagType { get; }
    }
}
