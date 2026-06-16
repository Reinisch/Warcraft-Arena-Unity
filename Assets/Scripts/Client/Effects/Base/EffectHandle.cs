namespace Client
{
    public readonly struct EffectHandle
    {
        private readonly IEffectEntity entity;
        private readonly long playId;

        public EffectHandle(IEffectEntity entity, long playId)
        {
            this.entity = entity;
            this.playId = playId;
        }

        public bool IsValid => entity != null && entity.IsPlaying(playId);

        public void Stop() => entity?.Stop(playId);
        public void Fade() => entity?.Fade(playId);
        public void Replay() => entity?.Replay(playId);
        public void ResetLocally() => entity?.ResetLocally(playId);

        public IEffectEntity Entity => entity;
    }
}