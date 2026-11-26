# CharmGoldFish Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, host settings, host requests, debug mode).

## The power: Lucky Catch

When the carrier is the first to discover a valuable, there is a `DuplicateChancePercent` chance (default 10) that the host creates a copy worth the original's current value. The Gold Fish is worn by `WearPercent` percent (default 100) of that value.

1. **Detect (carrier's machine).** `Patches/ValuableObject_Discover_Patch.cs` postfixes `ValuableObject.Discover`. The game calls it only on the machine of the player looking (`PhysGrabber.RayCheck`) and it returns true only for the first discovery (`State.Discover`; reminders return false).
2. **Report.** `GoldFishCharm.ReportDiscovery` checks `IsActive` and `RunIsLevel`, then uses CharmCore's `SendToHost`. On the host (or alone) the request carries the object itself (singleplayer view ids are all 0); a client sends the PhotonView id. CharmCore verifies the sender carries the charm.
3. **Roll (host).** `OnHostRequest` rejects dead or pocketed valuables, claims the valuable in `RolledValuables` (one chance per valuable and level, keyed by instance id) and rolls.
4. **Copy (host).** `ValuableCopier` finds the prefab by name in the Resources size folders (`Valuables/01 Tiny` to `07 Very Tall`), picks a free spot above or beside the original (line of sight to it, no overlap), protects the original from damage for 2 s, spawns it (`PhotonNetwork.InstantiateRoomObject`, or `Instantiate` alone) and sets `dollarValueOverride` before the new valuable reads its value. The game then sends the value to the clients and protects new objects from damage for a few seconds. The copy is claimed at once, so it never rolls.
5. **Feedback.** `CopyEffect` plays the game's teleport effect and unequip sound at the spot, on the host and, through Photon event 186, on every other machine.
6. **Pay.** `Domain/LuckyCatchMath.WearForCopy` turns the cost into a percentage of the fish's original value (capped at 100) and `Charm.WearValuable` applies it, so CharmCore's wear rules hold: wear handlers (the Ruben Doll) can reduce it and the fish is destroyed under 15% of its original value. If no copy could be created, nothing is paid.

`Domain/LuckyCatchMath.cs` is game-free and unit tested (`LuckyCatchMathTests`).

## Settings
| Key | Type | Range | Default | Meaning |
|---|---|---|---|---|
| `DuplicateChancePercent` | float | 0 to 100 | 10 | Chance per first discovery. 0 never, 100 always. |
| `WearPercent` (CharmCore, own description) | float | 0 to 100 | 100 | Share of the copy's value the fish pays. CharmCore's global `WearMultiplier` also applies. 0 makes copies free. |
| `Enabled` (CharmCore) | bool | | true | |

## Game members used

- `ValuableObject.Discover` (patched), `ValuableDiscoverGraphic.State`.
- `ValuableObject.dollarValueCurrent`, `dollarValueOriginal`, `dollarValueOverride`, `physGrabObject`.
- `PhysGrabObject.dead`, `OverrideIndestructible`.
- `AssetManager.instance`, `prefabTeleportEffect`, `soundUnequip`.
- `SemiFunc.IsMasterClientOrSingleplayer`, `IsMultiplayer`, `RunIsLevel`.

All are checked by `CharmGoldFish.Tests/GameContractTests.cs`.

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output.

## Known limits

- Not verified in game: the build, unit tests and contract tests prove the arithmetic and that the members exist, not the spawning, the placement or the multiplayer flow.
- Needs a CharmCore build that has `Charm.WearDescription`.
- A valuable is copied by prefab name from the size folders. Monster orbs and surplus bags live elsewhere and are skipped; a new size folder in a game update would silently stop copies for it (a debug line says so).
- The copy takes the original's current value as both its original and current value. Special state of a valuable (for example one that was already triggered) is not copied.
- The copy adds its value to the level's maximum haul figure shown in the truck, as any spawned valuable does; the extraction goal was already set.
- Big copies in tight places may find no free spot and then appear above the original.
- Everyone must have the mod: a client without it would not see the effect or be able to carry the fish.

## Decisions

- **Patch the discovery, not the camera:** the game already defines "you found it first" and scores it.
- **Host rolls:** one authority, and a repeated or forged request cannot roll twice.
- **Pay after spawning:** the fish never pays for a copy that does not exist.
- **Full copy even when the fish cannot pay:** the owner's rule; it is the jackpot of the power.
- **`WearPercent` reused as the cost share** instead of a second setting, with its own config description (`Charm.WearDescription`).
- **Own Photon event for the effect:** the game has no networked "flash here" call without side effects.

## Runtime hygiene
Checked against `RepoKit/references/runtime-hygiene.md`:
1. Lifetime of references: ok. Only instance ids are kept (`RolledValuables`), cleared in `OnLevelEnded`. No static field holds a game object; the Photon callback object holds none.
2. Work frequency: ok. Nothing runs per frame; everything is driven by a discovery or a Photon event.
3. Allocations: ok. The patch does nothing unless a first discovery happens; component and prefab lookups run once per successful copy.
4. UI: not applicable (no UI).
5. Patches: ok. The postfix exits on the result check; the rest runs once per discovered valuable.
