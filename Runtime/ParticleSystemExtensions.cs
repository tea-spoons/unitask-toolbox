using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
 
public static class ParticleSystemExtensions
{
    /// <summary>
    /// Plays the particle system and waits until it's done.
    /// </summary>
    public static async UniTask PlayAsync(this ParticleSystem particleSystem, bool withChildren, CancellationToken cancellationToken = default)
    {
        particleSystem.Play(withChildren);
 
        await particleSystem.WaitUntilDone(cancellationToken);
    }
 
    /// <summary>
    /// Plays the particle system and waits until it's done.
    /// </summary>
    public static async UniTask PlayAsync(this ParticleSystem particleSystem, CancellationToken cancellationToken = default)
    {
        await particleSystem.PlayAsync(true, cancellationToken);
    }
 
    /// <summary>
    /// Stops the <see cref="ParticleSystem"/> without clearing it, then waits until all remaining particles have expired.
    /// </summary>
    /// <remarks>If the <see cref="ParticleSystem"/> starts emitting again, the task is also finished.</remarks>
    public static async UniTask StopAsync(this ParticleSystem particleSystem, bool withChildren, CancellationToken cancellationToken = default)
    {
        particleSystem.Stop(withChildren, ParticleSystemStopBehavior.StopEmitting);

        await UniTask.WaitUntil(() => !particleSystem || particleSystem.isStopped || particleSystem.isEmitting, cancellationToken: cancellationToken);
    }
 
    /// <summary>
    /// Stops the <see cref="ParticleSystem"/> without clearing it, then waits until all remaining particles have expired.
    /// </summary>
    /// <remarks>If the <see cref="ParticleSystem"/> starts emitting again, the task is also finished.</remarks>
    public static async UniTask StopAsync(this ParticleSystem particleSystem, CancellationToken cancellationToken = default)
    {
        await particleSystem.StopAsync(true, cancellationToken);
    }
 
    /// <summary>
    /// Waits until the <paramref name="particleSystem"/> stops emitting, and all the particles have expired.
    /// </summary>
    public static async UniTask WaitUntilDone(this ParticleSystem particleSystem, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => !particleSystem || particleSystem.isStopped, cancellationToken: cancellationToken);
    }
}
