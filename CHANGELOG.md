# Changelog

## [Unreleased]

### Changed
- Runtime hygiene pass: the carrier, pending-wear and chased collections are cleared when a level ends; the monster patches exit first on a cheap carrier check; the carrier refresh no longer allocates a closure; debug lines in `Update` are guarded.

### Added
- Short Memory: monsters hunting a player who carries the Gold Fish lose track of them faster (`LoseTrackMultiplier`, default 2). Wears the valuable per second of being chased (default 2%/s).
- `Debug.DebugMode` (off by default): puts the Gold Fish on the floor at the start of each level and writes detailed logs.
