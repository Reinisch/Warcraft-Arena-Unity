using MagicOnion;

namespace ElleRealTimeStd.Shared.Test.Interfaces.Service
{
    public interface IMyFirstService : IService<IMyFirstService>
    {
        UnaryResult<int> SumAsync(int x, int y);
    }
}
