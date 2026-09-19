# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.unitask-toolbox` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- Depends on the public UniTask package (`com.cysharp.unitask`) instead of an internal copy.
- Made standalone: no longer declares UniTask as a dependency. Without UniTask installed the package compiles to nothing (its tests too).

## Planned changes

- [x] Tag and publish `v0.11.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
<!-- review-items:start -->
- [ ] **P0** Fix the `unity` field (or guard the newer APIs with `#if`) and confirm it by compiling on the lowest version that is declared.
- [ ] **P1** Decide the scope of `Tweens`: keep it as small helpers and document its limits, or provide optional adapters to LitMotion or PrimeTween. Add a zero-allocation test for the interpolation loops either way.
- [ ] **P1** Evaluate `Awaitable`-based versions of `StateMachine` and `ReusableCancelSource` for Unity 6 projects without UniTask.
- [ ] **P2** Add tests for the extensions that have none (`ParticleSystemExtensions`, `CoroutineExtensions`).
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Cysharp/UniTask](https://github.com/Cysharp/UniTask) | not checked | The library this package builds on. |
| [Guide for UniTask and Awaitable in Unity 6](https://github.com/Cysharp/UniTask/discussions/627) | discussion | Unity 6 has a built-in `Awaitable` with PlayerLoop awaits and cancellation, influenced by UniTask. Community comparisons say it has fewer combinators (for example `WhenAll`). |
| [annulusgames/LitMotion](https://github.com/annulusgames/LitMotion) | not checked | Zero-allocation tween library with a struct-based, data-oriented design. |
| [KyryloKuzyk/PrimeTween](https://github.com/KyryloKuzyk/PrimeTween) | not checked | Zero-allocation tween library. |

### Findings from reading the code

- **[Metadata]** `package.json` declares `unity: 2020.2`, but `AsyncInstantiateOperationExtensions` uses `AsyncInstantiateOperation` with no `#if`. That type does not exist in 2020.2, so the package would not compile there. `StateMachine` has one `#if UNITY_2022_1_OR_NEWER`.
- **[Overlap]** The `Tweens` helpers (position, scale, rect transform, curves) overlap with LitMotion and PrimeTween, which allocate nothing and have far more features.
- **[Coupling]** Everything needs UniTask. On Unity 6 a project may not want it.
<!-- review:end -->

## Notes and ideas

_Add your own here._
