
namespace TeaSpoons.UniTaskToolbox.Editor.Tests
{
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System;

    public class ReusableCancelSourceTest
    {
        [Test]
        public void ReusableCancelSourcePasses()
        {
            var source = new ReusableCancelSource();

            // Early cancel is fine.
            source.Cancel();

            // Fresh token is not cancelled.
            var token = source.Token;
            Assert.IsTrue(token.CanBeCanceled);
            Assert.IsFalse(token.IsCancellationRequested);

            // Cancelling cancels the previous token...
            source.Cancel();
            Assert.IsTrue(token.IsCancellationRequested);

            // ...but any token after that is not cancelled.
            token = source.Token;
            Assert.IsTrue(token.CanBeCanceled);
            Assert.IsFalse(token.IsCancellationRequested);

            // Disposing prevents getting the token afterwards.
            source.Dispose();
            Assert.Throws<ObjectDisposedException>(() => _ = source.Token);
        }
    }
}
