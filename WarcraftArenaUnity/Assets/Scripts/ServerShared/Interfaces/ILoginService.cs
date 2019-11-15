using System.Threading.Tasks;
using MagicOnion;

namespace ElleRealTimeStd.Shared.Test.Interfaces.Service
{
    public interface ILoginService : IStreamingHub<ILoginService, ILoginServiceReceiver>
    {
        //UnaryResult<int> CheckLogin(string username, string password);
        Task LeaveAsync();
        Task<int> JoinAsync(string roomName, string username, string password);
    }
}
