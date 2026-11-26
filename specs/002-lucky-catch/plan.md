# Plan: Lucky Catch

## Summary
Postfix on `ValuableObject.Discover` on the carrier's machine, a CharmCore host request, a host-side roll, copy and wear. Design in `ARCHITECTURE.md`.

## Technical Context
.NET Framework 4.8, BepInEx 5, Harmony, Photon PUN 2; Pockets and CharmCore. CharmCore gains `Charm.WearDescription`.

## Constitution Check
Host authority for rolls, spawns and wear; internal scoping; Tier 1 and Tier 2 tests; player-facing README. No RepoAPI module consumed.

## Known Pitfalls Check
- P-001 (item lists grow): not relevant. Copies are valuables, which are not registered in the persistent item lists.
- P-002 (shop battery): not relevant. No items are spawned.
- P-003 (view id 0 alone): relevant. The host path passes the object itself and remembers instance ids.
- P-004 (stale or locked DLL): relevant. Built with the game closed; deployed hashes compared.
- P-005 (extraction rooms): not relevant. Nothing is hidden; a copy inside the zone counts like any valuable.
- P-006 (hard-coded content lists): relevant. The seven size folders are listed; an unknown prefab is skipped and nothing is paid.
- P-007 (switched off mid-session): not relevant. No state is held; `Enabled` is checked on both machines before acting.
- P-008 (destroyed while held): relevant. The fish breaking in a pocket is handled by CharmCore and Pockets.
- P-009 (numeric limits): relevant. Ranges declared; the cost is capped again in `WearForCopy`.
- P-010 (long-lived references): relevant. Only ids are kept, cleared when the level ends; nothing per frame.

## Configuration Types Check
| Setting | Type | Unit | Valid | At the extremes |
|---|---|---|---|---|
| `DuplicateChancePercent` | float | percent | 0 to 100 | 0 never, 100 every first discovery |
| `WearPercent` | float | percent of the copy's value | 0 to 100 | 0 free copies, 100 exact price |

## Runtime Hygiene Check
See `ARCHITECTURE.md`, "Runtime hygiene".

## RepoAPI reuse decision
Stays in the mod: the copy logic is tied to this power. `ValuableCopier` (copy a valuable by prefab name) could be promoted if another mod needs to duplicate valuables.

## Project Structure
`GoldFishCharm.cs`, `ValuableCopier.cs`, `CopyEffect.cs`, `Patches/ValuableObject_Discover_Patch.cs`, `Domain/LuckyCatchMath.cs`.

## Complexity Tracking
None.
