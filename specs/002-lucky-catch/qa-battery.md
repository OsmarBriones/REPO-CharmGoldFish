# QA battery: Lucky Catch (CharmGoldFish)

Deploy: Pockets, CharmCore and CharmGoldFish are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmGoldFish.cfg` (off by default) and `DuplicateChancePercent = 100` in `Charm.GoldFish`, so every find is duplicated while testing. When a level starts, a Gold Fish is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. To get another one at any moment, set `SpawnNow = true` in the `Debug` section while a level is running: the valuable is placed near you and the switch turns itself back off (the file is re-read about once a second).

## Single player
- [ ] Log shows `Charm registered: Gold Fish - Lucky Catch` and `CharmGoldFish 1.0.0 loaded!`.
- [ ] Store the fish, look at a new valuable: a copy appears next to it with a flash and a sound, and it can be grabbed.
- [ ] The copy's price equals the original's; the fish loses that amount (log: `Lucky Catch: copied ...`).
- [ ] Discovering the copy does not create another one.
- [ ] Looking again at a valuable already discovered does nothing.
- [ ] Valuables discovered before storing the fish do nothing.
- [ ] A valuable worth more than the fish: the full copy appears, the fish breaks, the slot empties, no errors.
- [ ] The original loses no value when its copy appears.
- [ ] Big and tall valuables, and valuables in tight places or on shelves: the copy is reachable, not inside walls or floor, and nothing flies off or breaks.
- [ ] A monster orb is never duplicated.
- [ ] The copy counts toward the extraction when placed in the zone.
- [ ] Next level: the power works again; no errors on level change.

## Settings
- [ ] `DuplicateChancePercent = 0`: never copies. Default 10: occasional copies.
- [ ] `WearPercent = 50`: the fish pays half the copy's value. `0`: free copies.
- [ ] Charm `Enabled = false`: nothing happens. CharmCore `Enabled = false`: nothing works.
- [ ] With a Ruben Doll pocketed as well, the fish pays less and the doll pays too.

## Multiplayer (host and client)
- [ ] Client carries the fish and finds a valuable: the copy appears for everyone with the same price, both see the flash and hear the sound, the fish's value drops on both machines.
- [ ] A find by a player without the fish never creates a copy, even next to a carrier.
- [ ] A valuable the host already discovered, then looked at by the client carrier: nothing.
- [ ] Host settings override the client's.

Log patterns: `Charm registered`, `Lucky Catch`, `threw`, `Exception`.
