# Changelog

## [Unreleased]

### Changed
- The power is now Lucky Catch. It replaces Short Memory (monsters losing track of the carrier), which was removed before any release: it did too little and players could not tell it was happening.

### Added
- Lucky Catch: a valuable the carrier is the first to discover has a chance (`DuplicateChancePercent`, default 10%) to be duplicated. The Gold Fish loses as much value as the copy is worth (`WearPercent`, default 100% of the copy's value) and breaks when it cannot pay. A flash and a sound show every player where the copy appeared.
- `Debug.DebugMode` (off by default): puts the Gold Fish on the floor at the start of each level and writes detailed logs.
