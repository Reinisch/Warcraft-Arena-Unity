using System;
using Core;
using Cysharp.Threading.Tasks;
using Net;

namespace Client
{
    /// <summary>
    /// CLIENT-SIDE map lifecycle. Receives <see cref="LoadScenarioCommand"/> from the server and loads the
    /// matching scenario's scene locally (scene only, no server logic) so replicated shadows buffered in the
    /// World can materialise. On <see cref="EndScenarioCommand"/> it unloads its map and units.
    ///
    /// Inert on a host (it loaded the map itself and runs server logic); only a pure remote client acts.
    /// </summary>
    public sealed class ClientMapHandler : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly INetworkController controller;
        private readonly MapController mapController;
        private readonly BalanceReference balance;
        private readonly World world;
        private readonly IDisposable[] subscriptions;

        private int loadedScenarioIndex = -1;

        public ClientMapHandler(INetworkMessageBus bus, INetworkController controller,
            MapController mapController, BalanceReference balance, World world)
        {
            this.bus = bus;
            this.controller = controller;
            this.mapController = mapController;
            this.balance = balance;
            this.world = world;

            subscriptions = new[]
            {
                bus.Subscribe<LoadScenarioCommand>(OnLoadScenario),
                bus.Subscribe<EndScenarioCommand>(OnEndScenario),
            };

            // Once our own connection is established (handler registered, ready to receive), ask the server
            // which scenario to load. Pulling it this way is race-free vs the server pushing it on connect.
            controller.PeerConnected += OnConnected;
        }

        private void OnConnected(NetId self)
        {
            // The host loaded its own map and runs server logic; only a pure remote client needs to ask.
            if (world.HasServerLogic)
                return;

            // Fresh connection: the previous session's map was unloaded on leave, but loadedScenarioIndex
            // persists on this singleton. Clear it so a rejoin to the SAME scenario isn't skipped by the
            // idempotency guard in OnLoadScenario (which would leave the client mapless).
            loadedScenarioIndex = -1;
            bus.Send(new RequestScenarioCommand(), NetTarget.Server);
        }

        private void OnLoadScenario(LoadScenarioCommand msg, NetContext ctx)
        {
            // The host already has its own map + server logic; never react to its own broadcast.
            if (world.HasServerLogic)
                return;

            if (msg.ScenarioIndex < 0 || msg.ScenarioIndex >= balance.Scenarios.Count)
                return;

            if (msg.ScenarioIndex == loadedScenarioIndex)
                return;

            loadedScenarioIndex = msg.ScenarioIndex;
            mapController.LoadMapAsync(balance.Scenarios[msg.ScenarioIndex], unloadOthers: true, runScenario: false).Forget();
        }

        private void OnEndScenario(EndScenarioCommand msg, NetContext ctx)
        {
            if (world.HasServerLogic)
                return;

            loadedScenarioIndex = -1;
            mapController.UnloadAllAsync().Forget();
        }

        public void Dispose()
        {
            controller.PeerConnected -= OnConnected;

            for (int i = 0; i < subscriptions.Length; i++)
                subscriptions[i]?.Dispose();
        }
    }
}
