namespace Common
{
    public class SingleReference<T> : IReadOnlyReference<T> where T : class
    {
        public T Value { get; private set; }

        public SingleReference(T value)
        {
            Value = value;
        }

        public void Invalidate()
        {
            Value = null;
        }
    }
}
