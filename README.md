# UniTask Toolbox
Contains diverse tools to use with UniTask.

## AwaitableEvent
On top of regular `Action`s, you can register `Func<UniTask>`s to `AwaitableEvent`s.
The `Invoke` method is awaitable, so that every registered task has to be finished before the invocation is finished.

Notes:
- All registered tasks are run in parallel.
- Each registered response can throw an exception without interrupting the event invocation.
- Make the `AwaitableEvent` field private and add a public property that exposes its `Responses` property to make response addition and removal public,
while keeping the `Invoke` method private.
```csharp
private readonly AwaitableEvent onFoo = new();
public AwaitableEvent.ResponseProy OnFoo => onFoo.Responses;
```

## ReusableCancelSource
A reusable wrapper for `CancellationTokenSource`s.
Unlike with "bare" CTSs, you can use the `Token` and the `Cancel` method freely.
Calling `Cancel` cancels all previously given out `Token`s, and the next acquired `Token` will be from a fresh source.

`Cancel` only cancels when there's at least one uncancelled `Token` in circulation, so you can blindly use it.

Note: Make sure to `Dispose` the `ReusableCancelSource` when it's not needed anymore. For example in `OnDestroy`.
This will call `Cancel` and prevent any more `Token`s from being given out.

## AsyncProcess
A class that represents any async process, with callbacks for start end finish.
Allows calling code to decide whether they want to await the task result/finish or register a callback instead.

Available as non-generic and generic version.

## Tweens
Use the `Tweens` class to run an awaitable tween functions.

Refer to the `README.md` file in `Runtime/Tweens`.

## StateMachine
A simple state machine class, where each state is represented by an async method.

Usage example:
```csharp
private StateMachine stateMachine;

private void Awake()
{
    stateMachine = new(this);
}

private void Start()
{
    stateMachine.Start(Walk);
}

private async UniTask Walk()
{
    await Tweens.MoveTowards(transform, GetRandomTargetPosition(), 10f);

    stateMachine.SetNextState(Wait);
}

private async UniTask Wait()
{
    await UniTask.WaitForSeconds(5f);

    stateMachine.SetNextState(Walk);
}
```

## ParticleSystemExtensions
Extension methods for async `ParticleSystem` handling.
- `WaitUntilDone()`: Finishes when the `ParticleSystem` stopped and all remaining particles have expired.
- `PlayAsync()`: Plays the `ParticleSystem`, then awaits `WaitUntilDone()`.
- `StopAsync()`: Stops the `ParticleSystem`, then awaits `WaitUntilDone()`. Also finishes waiting if the `ParticleSystem` starts emitting again.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/unitask-toolbox.git
```

Pin a release by appending a tag, for example `#v0.10.5`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.cysharp.unitask` 2.5.0

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
