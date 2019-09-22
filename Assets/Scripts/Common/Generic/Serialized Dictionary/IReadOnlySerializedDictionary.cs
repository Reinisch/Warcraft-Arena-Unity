namespace Common
{
    public interface IReadOnlySerializedDictionary<in TKey, out TValue>
    {
        TValue Value(TKey key);
    }
}
