namespace Core
{
    public interface IUnitStateMachineBehaviour
    {
        void Register(UnitStateMachine stateMachine);
        void Unregister();
        void DoUpdate(int deltaTime);
    }
}
