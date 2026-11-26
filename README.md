# Charm Gold Fish

Lucky Catch: while the Gold Fish is in your pocket, every valuable you are the first to find has a chance to be duplicated, and the Gold Fish pays the price.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Gold Fish** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. Explore. When **you** discover a valuable nobody had found yet (the moment its outline and price appear), there is a 20% chance that a second one just like it pops up next to it, with a flash and a sound.
3. The Gold Fish loses as much value as the copy is worth. If the copy is worth as much as the fish or more, you still get the whole copy and the fish breaks.
4. Take the valuable out of the pocket and the power stops.

## Features

- **Lucky Catch:** a 20% chance of a duplicate each time you are the first to find a valuable. Finds made by your teammates do not count.
- **One chance per valuable:** each valuable gets a single try, and a copy can never be duplicated again.
- **The fish pays:** cheap copies slowly drain the fish; a very expensive find can be duplicated for the price of the whole fish. That is the big catch.
- **It breaks when nearly empty:** as with every worn-out valuable, the fish also breaks when less than 15% of its original value is left.
- **Everything is configurable:** the chance and how much the fish pays. The host's choices apply to everyone.

Example with a Gold Fish worth $1,500:

| You discover | Copy | The fish is left with |
|---|---|---|
| $900 vase | $900 | $600 |
| $10,000 statue | $10,000 | nothing, it breaks |

Monster orbs and the money bag left over after an extraction are never duplicated. A Ruben Doll in your pockets works on the Gold Fish too, so the fish pays only part of the price.

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
- `DuplicateChancePercent` (default `20`, from `0` to `100`): chance that a valuable you are the first to discover is duplicated.
- `WearPercent` (default `100`, from `0` to `100`): how much of the copy's value the Gold Fish pays. `100` means it loses exactly what was created, `50` half of it, `0` makes copies free.

Section `Debug`: `DebugMode` (default `false`) places a Gold Fish on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing. `SpawnNow` (default `false`, needs `DebugMode`): set it to `true` while a level is running to place one right away; it switches itself back off.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmGoldFish/issues)

## Credits
Developed by **Osmar Briones**
