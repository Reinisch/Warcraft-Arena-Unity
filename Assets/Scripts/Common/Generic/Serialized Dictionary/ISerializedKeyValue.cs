namespace Common
{
    public interface ISerializedKeyValue<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }
}
