# Warcraft Arena
World of Warcraft combat system implementation in Unity with Photon Bolt.

## Install
**Setup:**

Open project in Unity Editor with version specified in [ProjectSettings/ProjectVersion](ProjectSettings/ProjectVersion.txt).<br/>
Wait for asset import to finish then select "Launcher" scene in Assets/Scenes/Launcher then press "Play".<br/>

**Build:**

Game can only be launched from Launcher scene.<br/>
Launcher scene should be always included first, followed by other locations, currently only Lordaeron.<br/>

**Play:**

After running from editor in play mode or launching build, lobby panel will appear.<br/>
To create a server, select any map then press "Start Server" button.<br/>
To connect to existing server press "Start Client" then wait for sessions to appear and press connect.<br/>
Client is started by default, if connection fails then reason should be displayed at the bottom of lobby panel.<br/>

## Project status
**Done:**
- [X] Server instances and client connections
- [X] World of Warcraft player controller
- [X] World of Warcraft player camera
- [X] Basic location setup, player loading and spawning
- [X] Basic spells, spell effects, visuals and sounds
- [X] Lobby screen interface

**Next:**
- [ ] Spell system, new effect types
- [ ] Spell auras and buffs, aura application and management
- [ ] Arena or battleground logic
- [ ] Battle screen interface
- [ ] New characters, spells, locations, etc.

## Links
* [Photon Bolt](https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156) - networking solution used for server-client communication and game state synchronization. 
