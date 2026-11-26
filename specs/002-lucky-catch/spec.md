# Lucky Catch

Replaces `001-short-memory` (monsters losing track of the carrier), removed before any release because it did too little and players could not tell it was happening.

## User Scenarios

### User Story 1 - Lucky Catch (Priority: P1)
A player stores the Gold Fish in a pocket. Each valuable that player is the first to discover has a chance to be duplicated, and the Gold Fish loses the value that was created.

**Acceptance**:
- Only the carrier's own first discoveries count, once per valuable; a copy never rolls.
- On success a copy worth the original's current value appears next to it, with a flash and a sound for everyone.
- The Gold Fish loses the same amount; if the copy is worth as much as the fish or more, the full copy appears and the fish breaks.

## Requirements
- FR-001: Registers a charm with CharmCore for the prefab Valuable Museum GoldFish.
- FR-002: Settings, host-authoritative: `Enabled`, `DuplicateChancePercent` (0 to 100, default 10), `WearPercent` (share of the copy's value paid, 0 to 100, default 100). `DebugMode` and `SpawnNow` are local.
- FR-003: The host rolls, spawns the copy and applies the cost; a client only reports its discovery.
- FR-004: Monster orbs and extraction surplus bags are never duplicated.
- FR-005: All players install it (Category 2); depends on Pockets and CharmCore.
