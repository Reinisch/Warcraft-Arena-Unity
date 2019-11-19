# Warcraft Arena
World of Warcraft combat system implementation in Unity with Photon Bolt.

Feel free to ask anything about this project in [Discord](https://discord.gg/d62a5zG).

## Before development
Download [THIS](https://mega.nz/#!KtR3yIKC!ew9CDB8eLzVMhLCspUg7N8Sr-vRTifr7GYWQ85cdSSc) folder and extract it at the same level of Unity project.<br/>
Download [THIS](https://mega.nz/#!u9oRHarB!yaj9saQUh17H62go7-8PsRBpUreFhc_71lA6kozZ-wA) folder and extract it at "WarcraftArenaUnity\Assets\Plugins\Grpc.Core\runtimes".

## Setup to start developing
- Clone the repo
- Open with Unity3D "WarcraftArenaUnity" folder.
- Double click on a .cs file under the "Project" tab in unity. (ex. Assets/Scripts/InitClient.cs)
- If you modify/add an Interface in the ServerShared folder, make sure to generate/update some files by doing so: Click "MagicOnion" in the upper menù of Unity, then click "CodeGenerate". Check in the "Console" tab when it's finished.
- The starting scene is named **"LoginForm"** and you can find it under "Assets/Scenes"

## Start Playing
- .Net Standard 2.0 .Net Core 3 are required.
- Compile the solution with Visual Studio (i'm using Visual Studio 2019 Community edition).
- Execute .sql files inside Database folder. You can choose between MySql & SqlServer, execute their schema.sql and then execute InitData.sql
- Write config.json file with your data (it's inside "ElleRealTime/ElleRealTime/bin/Debug(or Release)/netcoreapp3.0/") [reference](https://github.com/LuigiElleBalotta/ElleRealTime/wiki/Configuration-file)
- Start the server console 
- Use your credential by creating an account (see server command [here](https://github.com/LuigiElleBalotta/ElleRealTime/wiki/Commands) or by using test/test or admin/admin)
- Enjoy

## Install
**Setup:**

Open project in **Unity Editor** with version specified in [ProjectSettings/ProjectVersion](ProjectSettings/ProjectVersion.txt).<br/>
Enter playmode from **LoginForm** scene in Assets/Scenes/LoginForm.<br/>

**Build:**

**Launcher** scene should be always included second (the first is LoginForm), followed by other locations, currently only Lordaeron.<br/>
Dedicated server may also be built using **Launcher Dedicated** scene.

> **Note**: For the moment the "bridge" between login and room selection is broken, in fact you can start play from **Launcher** > scene. We need to code:
> - Choose a room
> - ConnectAsync to that room
> - Spawn everything
> - Use **ElleRealTime** for networking and remove **Photon** (one day will be done).

**Play:**

To create a server, select any map then press **Start Server** button.<br/>
To connect to existing server press **Start Client** then wait for sessions to appear and press **Connect**.<br/>
Client is started by default, if connection fails then reason should be displayed at the bottom of lobby panel.<br/>

## Project status
**Done:**
- [X] Server instances and client connections with Photon Bolt
- [X] World of Warcraft player controller and camera
- [X] Arena location setup, Lordaeron map
- [X] Unit frames, action bars, custom hotkeys, lobby UI
- [X] Floating text, unit selection circle-projectors
- [X] Basic localization and spell error notifications
- [X] Spell, ui and character sounds
- [X] Visual effects for spell casts, projectiles and auras
- [X] Spells, spell effects, auras and aura effects

**Next:**
- [ ] More spells, effects and auras
- [ ] New spell and aura mechanics
- [ ] Arena or battleground logic

## Guides
* [How to add new spell](https://github.com/Reinisch/Warcraft-Arena-Unity/wiki/Adding-New-Spell)

## Screenshot
![Alt text](/Screenshots/WoW-Unity-1.0.48.png?raw=true "World of Warcraft Unity")

### Implemented spell effects:
| **Direct teleport**  | **Damage** | **Heal** | **Dispel Mechanics** |
| :---: | :---: | :---: | :---: |
| **Apply aura** | **Kill** | **Resurrect** | **Area Effects** |
### Implemented aura effects:
| **Absorb Damage**  | **Critical damage** | **Haste** | **Root** |
| :---: | :---: | :---: | :---: |
| **Display Model** | **Periodic healing** | **Confuse** | **Speed** |
| **Slow suppression** | **Periodic damage** | **Pacify** | **Silence** |
| **Ignore Aura State** | **Damage Reduction** | **Stat Mod** | **Immunity** |
| **Spell Modifier** | **Spell Trigger** | **Stun** | **Freeze** |

### Included spells:
| **Ice Block**  | **Renew** | **Ice Lance** | **Holy Word: Serenity** |
| :---: | :---: | :---: | :---: |
| **Ice Nova** | **Resurrect** | **Polymorph** | **Pain Suppression** |
| **Scorch** | **Periodic damage** | **Pacify** | **Silence** |
| **Icy Veins** | **Presence of Mind** | **Pyroblast** | **PvP Trinket** |
| **Flash Heal** | **Deep Freeze** | **Flame Strike** | **Cone of Cold** |
| **Counterspell** | **Ice Barrier** | **Frost Nova** | **Arcane Intellect** |
| **Living Bomb** | **Frost Bolt** | **Blazing Speed** | **Blink** |
| **Fire Blast** | **Hot Streak** | **Shatter** | **Fingers of Frost** |

### Controls:

**Hotkeys:**
Central Bottom Action Bar - **No Modifiers**, Central Top Action Bar - **Left Shift**, action buttons:

| S-1 | S-2 | S-3 | S-4 | S-5 | S-Q | S-E | S-R | S-F | S-Z | S-X | S-C | S-V | S-G |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **1** | **2** | **3** | **4** | **5** | **Q** | **E** | **R** | **F** | **Z** | **X** | **C** | **V** | **G** |

**Character:**
- Jump :              *Space*
- Move :              *WASD* or *Left + Right Click*
- Target :            *Tab* or *Left Click*
- Rotate Camera :     *Left Click* + *Drag*
- Rotate Character :  *Right Click* + *Drag*
- Select Self :       *F1*
- Chat :              *Enter*
- Toggle Healthbars : *Ctrl-V*
- Cancel Cast :       *Esc*
- Deselect Target :   *Esc*

## Links
* [Photon Bolt](https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156) - networking solution used for server-client communication and game state synchronization. 

## License
All character models, textures and sound are copyrighted by ©2004 Blizzard Entertainment, Inc. All rights reserved. World of Warcraft, Warcraft and Blizzard Entertainment are trademarks or registered trademarks of Blizzard Entertainment, Inc. in the U.S. and/or other countries.

The project itself is [GPL-3](LICENSE).
