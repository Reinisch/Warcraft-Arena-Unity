using ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player;

namespace ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub
{
    public interface IGamingHubReceiver
    {
        void OnJoin(Player player);
        void OnLeave(Player player);
        void OnMove(Player player);
    }
}
