# Warcraft Arena
World of Warcraft combat system implementation in Unity with Photon Bolt.

## Install
**Setup:**

Open project in **Unity Editor** with version specified in [ProjectSettings/ProjectVersion](ProjectSettings/ProjectVersion.txt).<br/>
Enter playmode from **Launcher** scene in Assets/Scenes/Launcher.<br/>

**Build:**

**Launcher** scene should be always included first, followed by other locations, currently only Lordaeron.<br/>
Dedicated server may also be built using **Launcher Dedicated** scene.

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

## Screenshot
![Alt text](/Screenshots/WoW-Unity-1.0.48.png?raw=true "World of Warcraft Unity")

### Implemented spell effects:
- Direct teleport
- Damage
- Heal
- Apply aura
- Kill
- Resurrect
- Area effects
- Dispel Mechanics

### Implemented aura effects:
- Absorb Damage
- Change Display Model
- Speed, critical damage and haste modifiers
- Pacify & Silence
- Periodic damage and healing
- Root
- Movement slow suppression
- Confuse
- Spell Modifier

### Included spells:
- Blazing Speed
- Blink
- Counterspell
- Fire Blast
- Flash Heal
- Frost Bolt
- Frost Nova
- Ice Barrier
- Ice Lance
- Icy Veins
- Living Bomb
- Polymorph
- Renew
- Resurrect
- Presence Of Mind
- Pyroblast
- Cone Of Cold
- Scorch

### Controls:

**Hotkeys:**
Central Bottom Action Bar - **No Modifiers**, Central Top Action Bar - **Left Shift**, buttons left to right:
1.  *1*
2.  *2*
3.  *3*
4.  *4*
5.  *5*
6.  *Q*
7.  *E*
8.  *R*
9.  *F*
10. *Z*
11. *X*
12. *C*
13. *V*
14. *G*

**Character:**
- Jump :              *Space*
- Move :              *WASD* or *Left + Right Click*
- Target :            *Tab* or *Left Click*
- Rotate Camera :     *Left Click* + *Drag*
- Rotate Character :  *Right Click* + *Drag*

## Links
* [Photon Bolt](https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156) - networking solution used for server-client communication and game state synchronization. 
