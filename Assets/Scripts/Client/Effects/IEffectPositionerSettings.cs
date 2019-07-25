namespace Client
{
    public interface IEffectPositionerSettings
    {
        bool AttachToTag { get; }
        bool KeepOriginalRotation { get; }
        bool KeepAliveWithNoParticles { get; }

        EffectTagType EffectTagType { get; }
    }
}
