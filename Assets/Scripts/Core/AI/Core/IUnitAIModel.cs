namespace Core
{
    public interface IUnitAIModel
    {
        void Register(UnitAI unitAi);

        void Unregister();

        void DoUpdate(int deltaTime);
    }
}
