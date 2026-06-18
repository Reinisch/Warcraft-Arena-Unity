using Client;
using Net.Local;
using Net.Ngo;
using Server;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class NetworkInstaller : Installer<NetworkInstaller>
    {
        public override void InstallBindings()
        {
            // Apply the network role to World logic flags before anything spawns.
            Container.BindInterfacesAndSelfTo<NetworkRoleInitializer>().AsSingle().NonLazy();

            // Single source of truth for the world/session lifecycle (start/join/leave + state gating).
            Container.BindInterfacesAndSelfTo<GameSession>().AsSingle();

            //Container.BindInterfacesAndSelfTo<LocalMessageBus>().AsSingle();
            //Container.BindInterfacesAndSelfTo<LocalNetworkController>().AsSingle();
            //Container.BindInterfacesAndSelfTo<LocalNetworkTime>().AsSingle();

            Container.BindInterfacesAndSelfTo<NgoMessageBus>().AsSingle();
            Container.BindInterfacesAndSelfTo<NgoNetworkController>().AsSingle();
            Container.BindInterfacesAndSelfTo<NgoNetworkTime>().AsSingle();

            Container.BindInterfacesAndSelfTo<NgoEntityRegistry>().AsSingle();
            Container.BindInterfacesAndSelfTo<NgoEntitySpawner>().AsSingle().NonLazy();

            // Server-side + client-side command handling (hosted in-process for single-player).
            Container.BindInterfacesAndSelfTo<ServerCommandRouter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientSpellResultHandler>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientChatHandler>().AsSingle().NonLazy();

            // Map lifecycle: server tells clients which scenario to load / when it's over; a remote client
            // loads its own scene (no server logic) so replicated shadows can materialise.
            Container.BindInterfacesAndSelfTo<ServerMapBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientMapHandler>().AsSingle().NonLazy();

            // Server→client outcome bridge (damage / heal / miss / hit). Server broadcasts Core events;
            // a remote client re-raises them locally. Dormant on a host (renders directly).
            Container.BindInterfacesAndSelfTo<ServerOutcomeBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientOutcomeHandler>().AsSingle().NonLazy();

            // Server→owning-client player movement (speed rate / teleport) the client can't derive itself.
            Container.BindInterfacesAndSelfTo<ServerPlayerBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientPlayerHandler>().AsSingle().NonLazy();

            // Arena scenario: server broadcasts match state/result; the client mirrors it for the HUD.
            Container.BindInterfacesAndSelfTo<ServerArenaBroadcaster>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientArenaController>().AsSingle().NonLazy();
        }
    }
}
