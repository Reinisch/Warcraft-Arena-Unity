using UnityEngine;

namespace Client
{
    public interface IEffectEntity
    {
        Transform Transform { get; }
        bool KeepOriginalRotation { set; }
        bool KeepAliveWithNoParticles { get; set; }

        bool IsPlaying(long playId);
        bool IsFading(long playId);
        void Fade(long playId);
        void Stop(long playId);
        void Replay(long playId);
    }
}
