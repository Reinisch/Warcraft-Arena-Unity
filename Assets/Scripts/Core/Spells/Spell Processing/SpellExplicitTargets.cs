namespace Core
{
    public class SpellExplicitTargets
    {
        public Unit Target { get; internal set; }

        public void Dispose()
        {
            Target = null;
        }
    }
}