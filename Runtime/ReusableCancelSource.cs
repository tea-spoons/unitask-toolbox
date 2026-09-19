
namespace TeaSpoons.UniTaskToolbox
{
    using System;
    using System.Threading;

    /// <summary>
    /// A wrapper for <see cref="CancellationTokenSource"/> that can be used for multiple cancellations.
    /// After each cancellation, the contained token source is replaced with a new one.
    /// </summary>
    public class ReusableCancelSource : IDisposable
    {
        private CancellationTokenSource source;
        private bool isDisposed = false;
        private bool currentSourceHasGivenOutToken = false;

        /// <summary>
        /// The current <see cref="CancellationToken"/> of this source.
        /// Will always be uncancelled at the time of receiving it.
        /// </summary>
        public CancellationToken Token
        {
            get
            {
                if (isDisposed) throw new ObjectDisposedException(GetType().Name);

                if (source == null)
                {
                    source = new CancellationTokenSource();
                }

                currentSourceHasGivenOutToken = true;
                return source.Token;
            }
        }

        /// <summary>
        /// Requests cancellation on all <see cref="Token"/>s that were given out since the last cancellation.
        /// </summary>
        /// <remarks>
        /// This method does not create garbage if no tokens were given out since the last cancellation,
        /// so it can be called liberally.
        /// </remarks>
        public void Cancel()
        {
            if (source != null && currentSourceHasGivenOutToken)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        public void Dispose()
        {
            Cancel();

            isDisposed = true;
        }
    }
}
