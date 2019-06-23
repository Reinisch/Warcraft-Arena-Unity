namespace Core
{
    public class SpellCastTargets
    {
        public Unit Target { get; internal set; }

        public void Dispose()
        {
            Target = null;
        }
    }
}