namespace Core
{
    public interface IUnitAIModel
    {
        void Register(Unit unit);

        void Unregister();

        void DoUpdate(int deltaTime);
    }
}
