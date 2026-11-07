# QA battery: Short Memory (CharmGoldFish)

Deploy: Pockets, CharmCore and CharmGoldFish are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmGoldFish.cfg` (off by default). When a level starts, a Gold Fish is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. Any level works.

## Single player
- [ ] Log shows `Charm registered: Gold Fish - Short Memory` and `CharmGoldFish 1.0.0 loaded!`.
- [ ] Store the valuable in a slot: the slot shows its picture and value.
- [ ] Without the fish in a pocket, let a monster chase you and hide: note how long it keeps hunting before it wanders off.
- [ ] With the fish in a slot, repeat the same chase: it gives up in about half the time (log with DebugMode: `Short Memory: monsters are chasing ...; wearing ...`).
- [ ] Hide where a monster looks under (crawl): it gives up searching sooner.
- [ ] The value drops about 2% of the original per second of being chased, and not at all while no monster is hunting you. Two monsters chasing at once wear at the same rate as one.
- [ ] While a monster keeps you in view, it chases as normal (no sudden early loss while you are seen).
- [ ] No cooldown veil appears on the slot.
- [ ] Take the valuable out: the power stops.
- [ ] Use it until the valuable breaks: the slot empties, the power stops, no errors in the log.

## Settings
- [ ] `WearPercent = 0`: monsters still forget faster, no wear. `LoseTrackMultiplier = 1`: no effect and no wear; `5`: very quick forgetting.
- [ ] Charm `Enabled = false`: no effect and no wear. CharmCore `Enabled = false`: nothing works.

## Multiplayer (host and client)
- [ ] A client carrying the fish: monsters hunting that client lose track faster, and the value drops on both machines.
- [ ] A player without the fish next to a carrier: monsters hunting the non-carrier behave as normal.
- [ ] Host settings override the client's.
- [ ] Client leaves and rejoins: no stuck veil.

Log patterns: `Charm registered`, `threw`, `Exception`.
