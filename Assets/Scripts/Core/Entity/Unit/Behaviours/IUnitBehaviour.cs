namespace Core
{
    internal interface IUnitBehaviour
    {
        void DoUpdate(int deltaTime);

        void HandleUnitAttach(Unit unit);

        void HandleUnitDetach();
    }
}
