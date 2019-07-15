namespace Core
{
    internal interface IUnitBehaviour
    {
        bool HasClientLogic { get; }
        bool HasServerLogic { get; }

        void DoUpdate(int deltaTime);

        void HandleUnitAttach(Unit unit);

        void HandleUnitDetach();
    }
}
