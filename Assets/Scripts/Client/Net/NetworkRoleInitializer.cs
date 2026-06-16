using Core;
using Net;

namespace Client
{
    /// <summary>
    /// Applies the active <see cref="NetworkRole"/> to the World's logic flags at startup, so behaviour
    /// gating (ILogicBehaviour) reflects whether this instance is host / dedicated server / remote client.
    /// Runs once on construction (NonLazy). In single-player the role is Host → both logics (unchanged).
    /// </summary>
    public sealed class NetworkRoleInitializer
    {
        public NetworkRoleInitializer(INetworkController controller, World world)
        {
            switch (controller.Role)
            {
                case NetworkRole.DedicatedServer:
                    world.ConfigureLogic(hasServerLogic: true, hasClientLogic: false);
                    break;
                case NetworkRole.RemoteClient:
                    world.ConfigureLogic(hasServerLogic: false, hasClientLogic: true);
                    break;
                default: // Host / None → run both (single-player default)
                    world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
                    break;
            }
        }
    }
}
