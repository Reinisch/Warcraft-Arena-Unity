namespace Client
{
    public interface IUnitRendererHandler
    {
        void HandleUnitRendererAttach(UnitRenderer attachedRenderer);
        void HandleUnitRendererDetach(UnitRenderer detachedRenderer);
    }
}
