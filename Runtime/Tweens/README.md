# Tweens
Use the `Tweens` class to run an awaitable tween functions.

The relevant methods are `Interpolate` (over duration) and `MoveTowards` (with speed).

Examples:
```csharp
// Basic syntax
await Tweens.Interpolate(from, to, duration, x => DoSomethingWith(x));
await Tweens.MoveTowards(from, to, speed, x => DoSomethingWith(x));

// Example: Color change
var sr = GetComponent<SpriteRenderer>();
await Tweens.Interpolate(sr.color, Color.red, 1f, color => sr.color = color);
```

Optional parameters of the two methods:
- `update`: Choose an update function (Update/FixedUodate/...).
- `curve`: An animation curve used for easing.
- `cancellation`: Pass a `Tweens.Cancellation` instance in order to be able to cancel the animation.

Find more information below.

## Parameters
### Parameter: update
An `UpdateFunction` returns a tuple `(float deltaTime, YieldAwaitable waitTask)`.

The system will call the update function between each animation frame. The returned `waitTask` will be awaited,
and the following animation update will use the returned `deltaTime`.

For convenience, these `UpdateFunction`s are ready to be used:
- `Tweens.Update` (default): Update loop.
- `Tweens.UpdateRealtime`: Update loop, but returns `Time.unscaledDeltaTime`.<br>Good for UI animations or anything else that ignores timescale.
- `Tweens.LateUpdate`: Update loop, but after the `LateUpdate` event.<br>Good for IK or other things that should happen after animation updates.
- `Tweens.FixedUpdate`: FixedUpdate loop.

### Parameter: curve
A standard Unity `AnimationCurve`. By default, no curve is applied, leading to a linear tween.

For convenience, a few common easing curves are predefined and ready to use. Find them in the `TweenCurves` class.

### Parameter: cancellation
A simple struct combining a `CancellationToken` and a flag `finishOnCancel`.

If `finishOnCancel` is set, the system will apply the `to` value opon cancelling.
If not, the callback will not be called anymore, leaving the tweened property in whatever state it is currently in.

It is recommended to not use the `Cancellation` constructor directly, but to instead do this:
```csharp
await Tweens.Interpolate(..., cancellation: cToken); // Pass the token directly to cancel without finishing.
await Tweens.Interpolate(..., cancellation: Tweens.Cancellation.FinishOn(cToken)); // Use "FinishOn" to get a final callback with the "to" value.
```

## Transform shorthands
For tweening transforms, some shorthands are available.

Examples:
```csharp
Tweens.Move(transform, targetPosition, duration);
Tweens.RotateTowards(transform, targetRotation, speed);
```
