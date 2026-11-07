# CharmGoldFish Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, cooldown display, host settings, debug mode).

## The power: Short Memory

While a player carries the Gold Fish, monsters hunting that player lose track of them `LoseTrackMultiplier` times as fast (default 2). It is a host effect: monster AI runs only on the master.

- `GoldFishCharm : Charm` registers the prefab `Valuable Museum GoldFish`, sets `WearsPerSecond = true` (default wear 2% per second, no cooldown) and the setting `LoseTrackMultiplier`.
- `ShortMemory` (a `MonoBehaviour`, host only) keeps a carrier cache (`CharmsApi.ForEachCarrier`, refreshed every 0.2 s), collects "player X was chased this frame" notes from the patches and accumulates wear per carrier in a dictionary; when at least 1% is pending it calls `charm.WearValuable(valuable, pending)`. Wear is per carrier per frame, so several monsters cost the same as one.
- `Patches/LoseTrackPatches.cs`: Harmony postfixes (applied with `PatchAll` in the plugin `Awake`). Each runs after the state's `Update`, only if `SemiFunc.IsMasterClientOrSingleplayer()`, `Enemy.MasterClient`, the enemy is still in that state, its `TargetPlayerAvatar` is a carrier and the charm is enabled. It subtracts a further `(multiplier - 1) x Time.deltaTime` from the countdown, clamped at 0.
- `Domain/LoseTrackMath.cs`: game-free `ExtraDecrease`, `Applies`, `TakeWear`; unit tested in `LoseTrackMathTests`.

### What is covered
| State | Timer sped up | Normal result when it reaches 0 |
|---|---|---|
| `EnemyStateChase` | `StateTimer` (private) | next `Update`: `StateTimer -= dt; <= 0` sets `ChaseSlow` |
| `EnemyStateChase` | `VisionTimer` (only while the target is not in view) | the monster stops tracking the live position and walks to the last seen point; `StateTimer` then ends the chase; no error at 0 (the code tests `> 0`) |
| `EnemyStateChaseSlow` | `StateTimer` (private) | sets `ChaseEnd`, then `Roaming` |
| `EnemyStateLookUnder` | `LookTimer` (private; only once `WaitDone`) | sets `ChaseSlow` |

### What is not covered (on purpose)
- `ChaseBegin` (the short wind-up before a chase), `ChaseEnd` (the monster already lost the target, it only pauses), `Roaming` and `Sneak`: they are not about holding on to a known player. `Investigate` is about a noise position, not a player.
- `LookUnder`'s `WaitTimer` (waiting at the hiding spot) is not sped up, only the search after it.
- Timers inside monster-specific scripts (custom AI beyond the shared states) are untouched.
- A monster that sees the player resets its timers as usual; the power shortens the countdowns, it does not blind monsters.

## Settings
| Key | Type | Range | Default | Meaning |
|---|---|---|---|---|
| `LoseTrackMultiplier` | float | 1 to 5 | 2 | Times faster the countdowns run. 1 = no effect (and no wear). Below 1 would make monsters hold on longer, so it is not offered; 5 is a sensible maximum. |
| `WearPercent` (CharmCore) | float | 0 to 100 | 2 | Percent of the original value per second a monster is actually chasing the carrier. |
| `Enabled` (CharmCore) | bool | | true | |

## Game members used

- `EnemyStateChase.Update`, fields `Enemy`, `StateTimer` (private), `VisionTimer`.
- `EnemyStateChaseSlow.Update`, fields `Enemy`, `StateTimer` (private).
- `EnemyStateLookUnder.Update`, fields `Enemy`, `LookTimer` (private), `WaitDone`.
- `Enemy.MasterClient`, `CurrentState`, `TargetPlayerAvatar`, `Vision`, `HasVision`; `EnemyVision.VisionTriggered`.
- `SemiFunc.IsMasterClientOrSingleplayer`, `PlayerAvatar.photonView`, `ValuableObject.dollarValueCurrent` (wear, done by CharmCore).

All are checked by `CharmGoldFish.Tests/GameContractTests.cs`.

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output.

## Known limits

- Looked up by exact prefab name; a game update that renames the prefab needs a mod update.
- Not verified in game: the build and unit tests prove the arithmetic and that the members exist, not the behaviour of the monsters. The patches have never run in the game.
- The carrier cache refreshes every 0.2 s, so picking up or dropping the fish takes up to that long to affect monsters.
- Only the host applies the effect; a carrier who is not the host is covered because Pockets tracks every player's pockets on every machine.

## Decisions

- **Postfix with an extra decrease:** the game's own decrease and transition logic stay untouched; clamping at 0 lets the game's own `<= 0` check in the next frame change state.
- **Wear per carrier, not per monster:** one note per player per frame, so a crowd of monsters does not multiply the wear.
- **Wear only when a timer was really sped up:** a timer already at 0, a visible target's sight timer or multiplier 1 cost nothing.
- **Sight timer left alone while the target is in view:** the game resets it every frame then; touching it would only create a one-frame flicker.
- **Own accumulator instead of `WearForSeconds`:** that helper works on the local player's `IsActive`; this effect runs on the host for any carrier.
- **Multiplier range 1 to 5:** no value below 1 (it would help monsters), no more than 5.

## Runtime hygiene
Checked against `RepoKit/references/runtime-hygiene.md`:
1. Lifetime of references: fixed. `ShortMemory` keeps dictionaries and sets keyed by `PlayerAvatar`/`ValuableObject` (carriers, pending wear, chased, scratch). `GoldFishCharm` now overrides `OnLevelEnded` and calls `ShortMemory.ClearAll()`, which empties all four collections and resets the refresh timer. No static field holds a game object (`ShortMemory.Instance` is the mod's own component, cleared in `OnDestroy`).
2. Work frequency: ok. The carrier refresh runs every 0.2 s; `Update` returns before any wear work when no monster chased a carrier this frame. Not the host: collections are cleared and nothing else runs.
3. Allocations: fixed. The refresh used a lambda that allocated a closure each time; it is now one delegate built in the constructor. The pending-wear cleanup is skipped when the dictionary is empty and the scratch list is emptied after use. The debug line in `Update` is guarded with `if (Charm.DebugMode)`. `CharmsApi.ForEachCarrier` (CharmCore) still allocates a player array and lists at 5 Hz; that is outside this mod. No `Find` calls.
4. UI: not applicable (no UI).
5. Patches: fixed. The three monster-state postfixes now test `ShortMemory.HasCarriers` (a count) first, so they exit immediately and allocate nothing when nobody carries the charm (and always on clients, where the cache is empty). The rest of the work is O(1) field reads.
Not verified in game (build and unit tests only).
