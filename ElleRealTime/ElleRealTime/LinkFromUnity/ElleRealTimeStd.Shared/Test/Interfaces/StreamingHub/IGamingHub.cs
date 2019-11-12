using System.Threading.Tasks;
using ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player;
using MagicOnion;
using UnityEngine;

namespace ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub
{
    public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
    {
        Task<Player[]> JoinAsync(string roomName, string userName, Vector3 position, Quaternion rotation);
        Task LeaveAsync();
        Task MoveAsync(Vector3 position, Quaternion rotation);
    }
}
