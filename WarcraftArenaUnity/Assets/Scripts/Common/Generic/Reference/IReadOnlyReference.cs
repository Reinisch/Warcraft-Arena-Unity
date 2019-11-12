namespace Common
{
    public interface IReadOnlyReference<out T>
    {
        T Value { get; }
    }
}
