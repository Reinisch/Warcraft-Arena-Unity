using Common;

namespace Client
{
    public abstract class SoundKit<TItem, TTypeKey> : ScriptableUniqueInfo<TItem> where TItem : ScriptableUniqueInfo<TItem>
    {
        public abstract SoundEntry FindSound(TTypeKey soundType, bool allowDefault);
    }
}
