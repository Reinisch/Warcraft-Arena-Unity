using System.Threading.Tasks;
using ElleRealTime.Core.BO;
using ElleRealTimeStd.Shared.Test.Interfaces.Service;
using MagicOnion.Server.Hubs;

namespace ElleRealTime.Services
{
    public class LoginService : StreamingHubBase<ILoginService, ILoginServiceReceiver>, ILoginService
    {
        IGroup room;

        public async Task LeaveAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> JoinAsync(string roomName, string username, string password)
        {
            Logger.Info($"Received {username} & {password}");

            (room) = await Group.AddAsync(roomName);

            var bo = new Login();
            int accountId = bo.CheckLogin(username, password);

            Program.Logger.Info($"Sending ID: {accountId}");

            Broadcast(room).OnJoin(accountId);

            return accountId;
        }
    }
}
