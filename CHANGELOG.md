# Changelog

## [0.1.9](https://github.com/Nagell/deduplicate/compare/v0.1.8...v0.1.9) (2026-04-01)


### Documentation

* simplify release installation instructions ([2621b3c](https://github.com/Nagell/deduplicate/commit/2621b3c427fdc27f4493050cceecd1ba9969817c))

## [0.1.8](https://github.com/Nagell/deduplicate/compare/v0.1.7...v0.1.8) (2026-04-01)


### Features

* bundle Windows App SDK runtime (self-contained deploy) ([1c94cfd](https://github.com/Nagell/deduplicate/commit/1c94cfdce0cbbca0fcdd129764d30302d85d0ce9))


### Documentation

* remove Windows App SDK runtime prerequisite (now self-contained) ([2dc24de](https://github.com/Nagell/deduplicate/commit/2dc24de644b7bb40ea31e2a2ed9c5876e90e8fd1))

## [0.1.7](https://github.com/Nagell/deduplicate/compare/v0.1.6...v0.1.7) (2026-04-01)


### Bug Fixes

* copy XBF files to publish output so WinUI XAML loads correctly ([1ea1500](https://github.com/Nagell/deduplicate/commit/1ea1500a26ffa03ec59427755c59252fcbbf07fb))

## [0.1.6](https://github.com/Nagell/deduplicate/compare/v0.1.5...v0.1.6) (2026-04-01)


### Bug Fixes

* include XBF files in publish output so app starts correctly ([c1d2504](https://github.com/Nagell/deduplicate/commit/c1d25049823a2bec28dbe16a9c28d5f3a1d3ca40))

## [0.1.5](https://github.com/Nagell/deduplicate/compare/v0.1.4...v0.1.5) (2026-04-01)


### Bug Fixes

* remove EnableMsixTooling — not needed without PublishSingleFile ([a986797](https://github.com/Nagell/deduplicate/commit/a986797d314affafba6bf4c54cc0ed95603b5bff))

## [0.1.4](https://github.com/Nagell/deduplicate/compare/v0.1.3...v0.1.4) (2026-04-01)


### Bug Fixes

* remove PublishSingleFile — unsupported for WinUI 3 unpackaged apps ([79cce52](https://github.com/Nagell/deduplicate/commit/79cce52ac6aadbfb9391d3fa6bbf2feb6a2e802b))

## [0.1.3](https://github.com/Nagell/deduplicate/compare/v0.1.2...v0.1.3) (2026-04-01)


### Bug Fixes

* enable MSIX tooling for single-file publish support ([52688d9](https://github.com/Nagell/deduplicate/commit/52688d9f97de953066ce8fafd8856d6a420ac970))

## [0.1.2](https://github.com/Nagell/deduplicate/compare/v0.1.1...v0.1.2) (2026-04-01)


### Bug Fixes

* abort deletion if scan restarts while confirmation dialog is open ([89d4d9d](https://github.com/Nagell/deduplicate/commit/89d4d9d7002cd6314e3e495a31b43f7a0ce0a580))

## [0.1.1](https://github.com/Nagell/deduplicate/compare/v0.1.0...v0.1.1) (2026-04-01)


### Features

* initial WPF/.NET 9 duplicate file finder app ([0ce712a](https://github.com/Nagell/deduplicate/commit/0ce712abf8f4e7641ee9506497747ac0a3dfc179))


### Bug Fixes

* address CTS race, dead state, async void safety, open-file/folder hardening, readonly DuplicateGroup.Items ([6c3ab8b](https://github.com/Nagell/deduplicate/commit/6c3ab8b449633fad8c5da57cbff244dcee728178))
* revert invalid compact constructor syntax in ScanProgress ([a70e903](https://github.com/Nagell/deduplicate/commit/a70e9034fcd49719493833221d13b7df17c72761))
* selection count and delete button styling in MainPage ([f6188d3](https://github.com/Nagell/deduplicate/commit/f6188d39682a93fdaffac201523e32eb48472be3))
* split status text onto separate lines ([b0dc510](https://github.com/Nagell/deduplicate/commit/b0dc51000da6eae59e4950a4615d6aa31d64fc38))
* surface file deletion failures and keep failed files in UI ([1ccbca9](https://github.com/Nagell/deduplicate/commit/1ccbca98b79e477db135c2914e3ab6855d9489db))
* track and surface files skipped during scan, replace MD5 with SHA-256 ([2a67fa5](https://github.com/Nagell/deduplicate/commit/2a67fa5e2ce3f468d95ac68dd0e970eb22d518a4))
* use supported release-please type 'simple' instead of 'dotnet' ([17f8589](https://github.com/Nagell/deduplicate/commit/17f85893958c33bef22d9f755599ad20e2d032ae))
* validate files before deletion — skip missing, reject size-changed ([8998868](https://github.com/Nagell/deduplicate/commit/89988680cf5bd1194123ae0329a36f576a72ed52))


### Documentation

* add Windows App SDK runtime prerequisite to README and release notes ([a8168e0](https://github.com/Nagell/deduplicate/commit/a8168e00d38ec6cd939910d5f40b1d1aa91a0be0))
* reorganize README and add DEVELOPMENT guide ([02503b7](https://github.com/Nagell/deduplicate/commit/02503b7d7288606c7a5aac11724e51a899db88d8))
