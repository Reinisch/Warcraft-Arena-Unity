namespace Core
{
    public class GridReference<TGridEntity> : Reference<GridReferenceManager<TGridEntity>, TGridEntity> where TGridEntity : class
    {
        public new GridReference<TGridEntity> Next => (GridReference<TGridEntity>)base.Next;


        public override void Initialize()
        {
        
        }

        public override void Deinitialize()
        {
            Unlink();
        }


        protected override void TargetObjectBuildLink()
        {
            // called from Link()
            Node = Target.AddFirst(this);
        }

        protected override void TargetObjectDestroyLink()
        {
            // called from Unlink()
        }

        protected override void SourceObjectDestroyLink()
        {
            // called from Invalidate()
        }
    }
}