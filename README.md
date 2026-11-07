# Charm Gold Fish

Short Memory: while the Gold Fish is in your pocket, monsters that chase you lose track of you twice as fast.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Gold Fish** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. That is all. Whenever a monster is hunting you, its chase runs out sooner: it gives up and goes back to wandering in about half the usual time, and it forgets faster where you hid.
3. Take the valuable out of the pocket and the power stops.

## Features

- **Short Memory:** monsters hunting you lose track of you twice as fast (you can change how much).
- **It costs the valuable:** every second a monster is chasing you wears away a share of its value (2% of the original by default), and a worn-out valuable breaks.
- **Everything is configurable:** wear, how much faster monsters forget you and anything specific to this power. The host's choices apply to everyone.

## Requirements
- [BepInEx Pack for R.E.P.O.](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
- [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/)
- [CharmCore](https://thunderstore.io/c/repo/p/OsmarBriones/CharmCore/)

## Installation
1. Install through r2modman / Thunderstore Mod Manager (Pockets and CharmCore come with it), or place `CharmGoldFish.dll` next to `Pockets.dll` and `CharmCore.dll` in `BepInEx/plugins`. **All players need it.**
2. Launch the game once; the configuration file is created inside `BepInEx/config`.

## Configuration
Settings are in `com.osmar.CharmGoldFish.cfg` inside `BepInEx/config`, section `Charm.GoldFish`. The host's choices apply to everyone.

- `Enabled` (default `true`): turn this charm on or off.
- `WearPercent` (default `2`): share of the valuable's original value lost each second a monster is chasing you while the power works. Several monsters chasing you at once still cost the same.
- `LoseTrackMultiplier` (default `2`, from `1` to `5`): how many times faster hunting monsters lose track of you. `1` means no effect.

Section `Debug`: `DebugMode` (default `false`) places a Gold Fish on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmGoldFish/issues)

## Credits
Developed by **Osmar Briones**

## Notes
The power works on monsters that are hunting you (chasing, following your trail, or searching where you hid). It does not stop monsters that merely hear a noise or wander, and a monster that sees you keeps chasing.
