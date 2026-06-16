# Warcraft Arena

A *World of Warcraft*–style combat system built in **Unity**, playable in **single-player**, over **LAN**, or on an **online server** — and featuring a standalone *Path of Exile* **Sirus** boss-fight game mode.

🎮 **Latest build:** [Download (Google Drive)](https://drive.google.com/drive/folders/1Nhwclml8pm-O0-mYWmbR8prcMiv3I7Gs?usp=drive_link)
💬 **Questions & feedback:** [Discord](https://discord.gg/d62a5zG)

![Warcraft Arena](/Screenshots/WoW-Unity-1.0.48.png?raw=true "Warcraft Arena in Unity")

---

## 🌍 Game Modes

- 🧍 **Single Player** — play solo and offline against AI and scripted encounters.
- 🖧 **LAN Server** — host or join a session on your local network to play with friends.
- 🌐 **Online Server** — play together over the internet (matchmaking via Unity Gaming Services Lobby + Relay).

## 🔥 Featured: Sirus Boss Fight (Path of Exile)

A **single-player boss encounter against Sirus, Awakener of Worlds** — the pinnacle boss from *Path of Exile*, recreated on top of the Warcraft Arena combat engine as a fast-paced bossfight shooter. Dodge his beams, survive the storm, and bring him down. ⚡

---

## ⚙️ Tech Stack

| Area | Technology |
| --- | --- |
| Engine | Unity 6 · Universal Render Pipeline (URP) |
| Networking | Unity **Netcode for GameObjects** (NGO) |
| Online services | **Unity Gaming Services** — Lobby + Relay |
| Dependency injection | **Zenject** |
| Async | **UniTask** |
| AI | **Unity Behavior** (behavior graphs) + AI Navigation (NavMesh) |
| Input | Unity **Input System** |
| Content | **Addressables** · **Localization** · TextMeshPro / uGUI |

> ℹ️ **Networking history:** This project originally used **Photon Bolt**. That backend has been **deprecated** and preserved on the [`photon-bolt`](https://github.com/Reinisch/Warcraft-Arena-Unity/tree/photon-bolt) branch. The `master` branch now runs on Unity Netcode for GameObjects behind a framework-agnostic networking layer (see [Architecture](#-architecture)).

## 🏗️ Architecture

The codebase is split into assemblies (`.asmdef`) with a strict dependency direction, so gameplay logic never depends on any networking framework:

```
Game.Core  ──────────────►  Game.Common
   │  (pure gameplay: units, spells,
   │   auras, combat — no net deps)
   ▼
Game.Net.Abstraction  ◄── framework-neutral interfaces & messages
   ├── Game.Net.Local  (single-player / offline backend)
   └── Game.Net.Ngo    (Netcode for GameObjects backend)

Game.Server   ── authoritative server logic
Game.Client   ── presentation, split into:
   ├── Game.Client.UI          (unit frames, action bars, lobby, chat)
   ├── Game.Client.Effects     (spell/projectile/aura VFX)
   ├── Game.Client.Sound       (spell, UI and character audio)
   └── Game.Client.Localization
Game.Launcher ── bootstrap, DI installers, scene flow
Game.Editor   ── editor tooling
```

The networking framework lives **only** in adapter assemblies (`Game.Net.Ngo`, `Game.Net.Local`). `Game.Core` references just `Game.Common`, and the asmdef dependency graph enforces that wall at compile time — making the netcode backend swappable.

## ⚔️ Combat System

Combat is fully **data-driven**. Content consists of reusable ScriptableObject *Balance Definitions*:

- **Spells** are assembled from one or more **Spell Effects** (the things a cast *does*).
- **Auras** are assembled from one or more **Aura Effects** (the persistent buffs/debuffs a spell *applies*).

This makes new abilities largely a matter of authoring assets — see the wiki guide: **[How to add a new spell](https://github.com/Reinisch/Warcraft-Arena-Unity/wiki/Adding-New-Spell)**.

### Spell kit

A *World of Warcraft* Mage kit (plus a few support spells), grouped by school:

| 🔥 Fire | ❄️ Frost | 🔮 Arcane / Utility | ✨ Support |
| --- | --- | --- | --- |
| Pyroblast | Frost Bolt | Polymorph | Flash Heal |
| Scorch | Ice Lance | Blink | Renew |
| Flame Strike | Frost Nova | Counterspell | Holy Word: Serenity |
| Living Bomb | Cone of Cold | Arcane Intellect | Pain Suppression |
| Fire Blast | Deep Freeze | Presence of Mind | Resurrect |
| Blazing Speed | Ice Barrier · Ice Block | PvP Trinket | |
| Hot Streak *(proc)* | Icy Veins · Ice Nova | | |
| | Fingers of Frost · Shatter | | |

<details>
<summary><b>Spell Effects</b> — what a cast can do</summary>

Damage · Heal · Apply Aura · Area Effects · Direct Teleport · Dispel Mechanics · Kill · Resurrect

</details>

<details>
<summary><b>Aura Effects</b> — buffs &amp; debuffs auras can apply</summary>

Periodic Damage · Periodic Healing · Absorb Damage · Damage Reduction · Critical Damage · Haste · Speed · Stat Mod · Spell Modifier · Spell Trigger · Stun · Freeze · Root · Silence · Pacify · Confuse · Immunity · Slow Suppression · Ignore Aura State · Display Model

</details>

## ✨ Implemented Features

- **Networking** — server instances and client connections via Netcode for GameObjects, with single-player, LAN, and online (Relay) backends behind a shared abstraction.
- **Player** — *World of Warcraft*–style player controller and third-person camera.
- **World** — arena location setup (Lordaeron map) and the Sirus boss-fight scenario.
- **Combat** — data-driven spells, spell effects, auras and aura effects (see above).
- **UI** — unit frames, action bars, customizable hotkeys, lobby, chat, floating combat text, unit selection projectors.
- **Presentation** — visual effects for casts, projectiles and auras; spell, UI and character sounds.
- **Systems** — localization with spell-error notifications; AI behavior graphs for scripted encounters.

## 🚀 Getting Started

**Setup**

Open the project in the **Unity Editor** version specified in [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt). Enter play mode from the **Launcher** scene (`Assets/Scenes/Launcher`).

**Build**

The **Launcher** scene must be included first, followed by the playable locations.

**Play**

From the lobby:
- **Single Player** — start a local session and play offline.
- **Start Server** — host a session, advertised for LAN/online clients to discover.
- **Start Client** → wait for sessions to appear → **Connect**. If a connection fails, the reason is shown at the bottom of the lobby panel.

## 🎮 Controls

**Action bars** — Central Bottom bar: *no modifier* · Central Top bar: *Left Shift*

| 1 | 2 | 3 | 4 | 5 | Q | E | R | F | Z | X | C | V | G |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |

**Character**

| Action | Input |
| --- | --- |
| Move | *WASD* or *Left + Right Click* |
| Jump | *Space* |
| Target | *Tab* or *Left Click* |
| Select self | *F1* |
| Deselect / cancel cast | *Esc* |
| Rotate camera | *Left Click* + drag |
| Rotate character | *Right Click* + drag |
| Toggle health bars | *Ctrl + V* |
| Chat | *Enter* |

---

## 📜 License & Attribution

All character models, textures and sounds are copyrighted by ©2004 Blizzard Entertainment, Inc. All rights reserved. *World of Warcraft*, *Warcraft* and *Blizzard Entertainment* are trademarks or registered trademarks of Blizzard Entertainment, Inc. in the U.S. and/or other countries.

*Path of Exile* and *Sirus, Awakener of Worlds* are trademarks of Grinding Gear Games.

This is a non-commercial, educational fan project and is not affiliated with or endorsed by Blizzard Entertainment or Grinding Gear Games.
