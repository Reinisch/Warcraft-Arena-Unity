using UnityEngine;

namespace Client.Sound
{
    public readonly struct SoundPlayHandle
    {
        private readonly long playId;
        private readonly bool isValid;
        private readonly bool scheduleRelease;
        private readonly SoundLoadHandle loadHandle;
        private readonly SoundPlayController controller;

        public long PlayId => playId;
        public bool ScheduleRelease => scheduleRelease;
        public AudioSource Source => controller?.GetSource(playId);
        public bool IsValid => isValid && controller.IsValid(playId);
        public bool IsPlaying => isValid && controller.IsPlaying(playId);

        internal SoundPlayHandle(long playId, bool scheduleRelease, SoundLoadHandle loadHandle, SoundPlayController controller)
        {
            this.playId = playId;
            this.controller = controller;
            this.loadHandle = loadHandle;
            this.scheduleRelease = scheduleRelease;
            isValid = true;
        }
        
        public void Loop(float startTime, float endTime)
        {
            if (IsPlaying && Source.time >= endTime)
                Source.time = startTime;
        }

        public void Release()
        {
            if (IsValid)
            {
                controller.ReleasePlay(this);
                controller.ReleaseLoad(loadHandle);
            }
        }
    }
}
