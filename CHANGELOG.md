## [0.11.1]

- No code changes. Independent development continues from this release, outside of Bigpoint; 0.11.0 was the last version developed there.

## [0.11.0]

- Made standalone: no longer declares UniTask as a dependency. Without UniTask installed the package compiles to nothing (its tests too).

## [0.10.0] - 2025-11-19
- Added AsyncProcess

## [0.9.3] - 2025-05-26
- Added StateMachine.Start(Action) overload
- Made StateMachine an IDisposable

## [0.9.2] - 2025-05-26
- Added StateMachine.SetNextState(Action) overload 

## [0.9.1] - 2025-05-26
- Fixed critical StateMachine bug

## [0.9.0] - 2025-05-23
- Added StateMachine class

## [0.8.1] - 2025-04-10
- Added AsyncInstantiateOperationExtensions (replaces InstantiateUtility)

## [0.8.0] - 2025-04-10
- Added InstantiateUtility

## [0.7.3] - 2025-04-09
- Implemented silent failing for NREs in the final callback in a tween

## [0.7.1] - 2025-04-02
- Fixed starting values in RectTransformTweens

## [0.7.0] - 2025-04-02
- Added RectTransform-specific tweening functions

## [0.6.1] - 2025-04-02
- Made ReusableCancelSource.Cancel safer to use

## [0.6.0] - 2025-04-02
- Added ReusableCancelSource

## [0.5.1] - 2025-03-21
- Added default values to update parameter in TransformTweens to allow omitting it in calls

## [0.5.0] - 2025-02-13
- Added AwaitableEvent.Responses, splitting access between responses and invocation

## [0.4.0] - 2025-02-07
- Added UniTask-based tweening system

## [0.3.0] - 2025-01-17
- Added AwaitableEvent.Clear

## [0.2.0] - 2024-01-08
- Added ParticleSystemExtensions

## [0.1.0] - 2023-12-20
- Added AwaitableEvent
