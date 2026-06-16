namespace Client.Sound
{
    public readonly struct SoundLoadHandle
    {
        private readonly SoundEntry entry;
        private readonly SoundPlayController controller;
        private readonly bool single;
        private readonly long loadId;
        
        public SoundEntry Entry => entry;
        public bool Single => single;
        public long LoadId => loadId;

        internal SoundLoadHandle(SoundEntry entry, SoundPlayController controller, bool single, long loadId)
        {
            this.entry = entry;
            this.controller = controller;
            this.single = single;
            this.loadId = loadId;
        }

        public void Release()
        {
            controller?.ReleaseLoad(this);
        }
    }
}
