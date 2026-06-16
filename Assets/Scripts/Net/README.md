# Networking abstraction (`Net.*`)

Goal: **Core never references a networking framework.** Gameplay code talks to framework-neutral
interfaces in `Game.Net.Abstraction`; a swappable adapter assembly implements them. Today the only
adapter is `Game.Net.Local` (a dummy in-process one for single-player / host); a real adapter
(FishNet / Fusion / NGO / …) drops in later without touching callers.

This replaces the old project's Photon Bolt integration (`D:\Projects\Unity\Warcraft-Arena-Unity`),
which leaked Bolt types deep into Core (`EntityBehaviour`, `IState`, `IProtocolToken`, global/entity
events). Here, the equivalents live behind interfaces and as plain message/token data.

## Assemblies

| Assembly | References | Role |
|---|---|---|
| `Game.Net.Abstraction` | `Game.Core`, `UniTask` | The contract: interfaces, routing types, message structs, tokens, state snapshots. No framework. |
| `Game.Net.Local` | `Game.Net.Abstraction`, `UniTask` | Dummy single-player/host adapter. In-process bus, no real connection. |
| (future) `Game.Net.<Framework>` | + the framework | Real adapter: shadow `NetworkBehaviour`, transport, RPC mapping. |

`Game.Core` depends on **neither** — that's the enforced compile wall. The adapter is wired in DI
([CoreInstaller.cs](../../Launcher/Installers/CoreInstaller.cs)); swap the bindings to go online.

## The six seams (`Game.Net.Abstraction`)

- **`INetworkController`** — lifecycle: `StartHost` / `StartServer` / `StartClient` / `Connect` /
  `Shutdown` + connection events. Host / DedicatedServer / RemoteClient (`NetworkRole`) is the only
  difference between "client-as-server", "server only" and "remote client".
- **`INetworkTime`** — `ServerFrame` (replaces `BoltNetwork.ServerFrame`).
- **`INetworkEntity` / `INetworkEntityListener`** — entity identity (`NetId`, `IsOwner`,
  `IsController`) + attach/detach/control callbacks (replaces `EntityBehaviour`).
- **`INetState<T>`** — replicated state snapshot + change notification (replaces `GetState<T>()` +
  `AddCallback`). Driven by the shadow component in a real adapter.
- **`INetworkMessageBus`** — `Send<T>(msg, NetTarget, NetReliability)` + `Subscribe<T>`. One seam for
  all the old Bolt events.
- **`INetWriter` / `INetReader` / `INetSerializable`** — framework-neutral serialization (replaces
  `UdpPacket` / `IProtocolToken`).

Routing: `NetTarget` (Server / AllClients / Everyone / a specific `Connection` / `EntityObservers`
filtered by `EntityScope`) unifies Bolt's `GlobalTargets` + `EntityTargets`.

## Messages & tokens

- **Messages** (`Messages/`): 21 plain `INetMessage` structs — Requests (client→server),
  Notifications (server→client), Entity (entity-scoped). Serialization is the adapter's job.
- **Tokens** (`Tokens/`): 3 `INetSerializable` connect payloads (ServerRoom / ClientConnection /
  ClientRefuse) — self-serialize via `INetWriter`/`INetReader`. (The per-cast `SpellProcessingToken`
  is Core's own type, carried directly by `UnitSpellLaunch`.)
- **State** (`State/`): `PlayerState` (complete), `UnitState` (**seed only** — the full replicated
  field set is enumerated against the new `Unit` in the entity-binding step).

## Command flow (current demo)

Player input no longer calls Core directly for commands; it sends a message:

```
InputReference (client)
  └─ messageBus.Send(new SpellCastRequest(spellId, flags), NetTarget.Server)
        │  (Game.Net.Local: delivered synchronously in-process)
        ▼
ServerCommandRouter (Client/Net, "server" logic for now)
  └─ world.PlayerManager.Player.Spells.CastSpell(...)        // the original gameplay call
  └─ messageBus.Send(new SpellCastResultNotification(...), NetTarget.To(sender))
        ▼
ClientSpellResultHandler (Client/Net)
  └─ on failure → eventBus.ExecuteEvent(GameEvents.ClientSpellFailed, result)   // unchanged feedback
```

Because the local bus dispatches synchronously, single-player behaves exactly as before.

### Routed through the bus today
**All player commands**: `CastSpell`, `CastSpellWithDestination`, `CastSpellWithTargetingOptions`,
`StopCasting`, `DoEmote`, `SwitchClass`, `SelectTarget`, chat (`Say`).

Entity-referencing commands resolve `NetId`s through **`INetEntityRegistry`** (`WorldEntityRegistry`
in single-player: `NetId` == the existing `Entity.Id`, via `UnitManager.TryFind`).

**Server→client outcomes** — the `ServerOutcomeBroadcaster` (`Game.Server`) → `ClientOutcomeHandler`
(`Game.Client`) bridge: spell **damage / heal / miss / hit / launch**. The server re-broadcasts Core's
gameplay events as messages; a *remote* client re-raises them as the same local Core events so existing
renderers/sound are untouched. Dormant on a host (renders directly) to avoid double-fire.

### Still direct / not yet over the bus
- **Player movement** — stays local/direct by design (player controls movement freely).
- **Entity state** (health/power/flags/model/scale/class/faction/target/auras, emote display,
  teleport) — these are continuous **state**, not one-shot events; they need the shadow / state-
  replication layer in the real adapter, not the message bus.

## Done so far
Abstraction contracts; dummy local adapter; **all player commands routed**; entity `NetId` binding
(`INetEntityRegistry`); role/behaviour gating (`World.HasServerLogic`/`HasClientLogic` +
`ILogicBehaviour` — a remote client won't run server combat/aura/AI logic); server→client outcome
bridge; **server logic split into `Game.Server`** (`ServerCommandRouter`, `ServerOutcomeBroadcaster`).
Client-side glue (`Client*Handler`, `WorldEntityRegistry`, `NetworkRoleInitializer`) stays in
`Game.Client`.

## Live adapter: Netcode for GameObjects (`Game.Net.Ngo`)
NGO is the active adapter (`NgoMessageBus`/`NgoNetworkController`/`NgoNetworkTime` bound in
`NetworkInstaller`; `Local*` commented out). Shadows are **separate** `NetworkObject` prefabs
(`EntityNetworkView`) carrying a `NetworkVariable<NgoUnitSnapshot>`; the Core unit prefabs stay
framework-free. `NgoEntitySpawner` (server) reacts to `UnitManager.EventEntityAttached/Detach` and
spawns/despawns a matching shadow; `NgoEntityRegistry` is the live `INetEntityRegistry`
(`NetId = NetworkObjectId`). **Host mode verified** (spells/chat/emotes).

### Client map lifecycle
A remote client loads its **own** scene (no server logic) so replicated shadows can materialise:
- `ServerMapBroadcaster` (server) → `LoadScenarioCommand{ScenarioIndex}` on map load + to late joiners,
  `EndScenarioCommand` on unload. Index = position in `BalanceReference.Scenarios` (stable on all peers).
- `ClientMapHandler` (pure client) → `LoadMapAsync(scenario, unloadOthers, runScenario:false)` (scene
  only, scenario graph skipped) / `UnloadAllAsync`.
- `EntityNetworkView` **buffers** shadows in `World` until `MapController.EventMapLoaded`.
- Scenario entity-spawns are server-only (`RunScenarioSetupActionsAction` gated on `HasServerLogic`).

## Not yet done / roadmap
1. **Ownership**: shadows are server-owned, so a remote client's own player isn't `PlayerManager.Player`
   and a fresh client has no player to control / follow with the camera. Needs per-connection player +
   ownership flow.
2. **Continuous state replication**: transform/health/power/auras/etc. beyond the one-shot spawn
   snapshot (ongoing `ApplyState`). Plus `UnitSpellLaunch` token bridge and Relay/Lobby for `ConnectAsync`.
3. **Durability**: a Core serializer (same format as save/load) for reconnect / restart-with-state.
