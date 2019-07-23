using UnityEngine;

namespace Client
{
    public interface IEffectEntity
    {
        Transform Transform { get; }

        bool IsPlaying(long playId);
        void Stop(long playId);
        void Replay(long playId);
    }
}
