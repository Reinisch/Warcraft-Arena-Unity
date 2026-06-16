using System;
using Core;
using Net;

namespace Server
{
    /// <summary>
    /// Server-authoritative map lifecycle bridge. Tells clients which scenario map to load (and when the
    /// map is over) so they can load their own scene locally — no server logic crosses the wire, only the
    /// scenario index into <see cref="BalanceReference.Scenarios"/>.
    ///
    /// On map load it broadcasts <see cref="LoadScenarioCommand"/> to all clients and remembers the active
    /// scenario so late-joining clients (<see cref="INetworkController.PeerConnected"/>) get caught up.
    /// On map unload it broadcasts <see cref="EndScenarioCommand"/>. Inert when this instance has no server
    /// logic (a pure client).
    /// </summary>
    public sealed class ServerMapBroadcaster : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly INetworkController controller;
        private readonly MapController mapController;
        private readonly BalanceReference balance;
        private readonly World world;
        private readonly IDisposable scenarioRequestSub;

        private int activeScenarioIndex = -1;

        public ServerMapBroadcaster(INetworkMessageBus bus, INetworkController controller,
            MapController mapController, BalanceReference balance, World world)
        {
            this.bus = bus;
            this.controller = controller;
            this.mapController = mapController;
            this.balance = balance;
            this.world = world;

            mapController.EventMapLoaded += OnMapLoaded;
            mapController.EventMapUnloaded += OnMapUnloaded;
            controller.PeerConnected += OnPeerConnected;
            scenarioRequestSub = bus.Subscribe<RequestScenarioCommand>(OnScenarioRequested);
        }

        // A client asked (once fully connected) which scenario to load — reply just to it. This is the reliable
        // path: an unsolicited LoadScenarioCommand on connect can lose the race against the client's handler
        // being ready over Relay; a reply to the client's own request cannot.
        private void OnScenarioRequested(RequestScenarioCommand msg, NetContext ctx)
        {
            if (!world.HasServerLogic || activeScenarioIndex < 0)
                return;

            bus.Send(new LoadScenarioCommand(activeScenarioIndex), NetTarget.To(ctx.Sender));
        }

        private void OnMapLoaded(Map map)
        {
            if (!world.HasServerLogic)
                return;

            int index = IndexOfScenario(map.Scenario);
            if (index < 0)
                return;

            activeScenarioIndex = index;
            bus.Send(new LoadScenarioCommand(index), NetTarget.AllClients);
        }

        private void OnMapUnloaded(Map map)
        {
            if (!world.HasServerLogic)
                return;

            // Only the primary/active map drives the client's map lifecycle for now.
            if (IndexOfScenario(map.Scenario) != activeScenarioIndex)
                return;

            activeScenarioIndex = -1;
            bus.Send(new EndScenarioCommand(), NetTarget.AllClients);
        }

        // A client connecting after the map already loaded still needs to know what to load.
        private void OnPeerConnected(NetId peer)
        {
            if (!world.HasServerLogic || activeScenarioIndex < 0)
                return;

            bus.Send(new LoadScenarioCommand(activeScenarioIndex), NetTarget.To(peer));
        }

        private int IndexOfScenario(ScenarioDefinition scenario)
        {
            if (scenario == null)
                return -1;

            for (int i = 0; i < balance.Scenarios.Count; i++)
                if (balance.Scenarios[i] == scenario)
                    return i;

            return -1;
        }

        public void Dispose()
        {
            mapController.EventMapLoaded -= OnMapLoaded;
            mapController.EventMapUnloaded -= OnMapUnloaded;
            controller.PeerConnected -= OnPeerConnected;
            scenarioRequestSub?.Dispose();
        }
    }
}
