namespace Core
{
    public class FollowerReference : Reference<Unit, ITargetedMovementGenerator>
    {
        protected override void TargetObjectBuildLink() { }
        protected override void TargetObjectDestroyLink() { }
        protected override void SourceObjectDestroyLink() { }
    }
}