// Origin: https://gist.github.com/wildbook/5025d6f9688f08a7cf1d4b9f75bb106c

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Currycomb.Common
{
    public class SemaphoreSlimGuarded : SemaphoreSlim
    {
        public SemaphoreSlimGuarded(int initialCount) : base(initialCount) { }
        public SemaphoreSlimGuarded(int initialCount, int maxCount) : base(initialCount, maxCount) { }

        public Guard GuardedWait()
        {
            this.Wait();
            return new Guard(this);
        }

        public Guard GuardedWait(CancellationToken cancellationToken)
        {
            this.Wait(cancellationToken);
            return new Guard(this);
        }

        public async Task<Guard> GuardedWaitAsync()
        {
            await this.WaitAsync();
            return new Guard(this);
        }

        public async Task<Guard> GuardedWaitAsync(CancellationToken cancellationToken)
        {
            await this.WaitAsync(cancellationToken);
            return new Guard(this);
        }

        public Guard? GuardedWait(int millisecondsTimeout)
            => this.Wait(millisecondsTimeout) ? new Guard(this) : null;
        public Guard? GuardedWait(int millisecondsTimeout, CancellationToken cancellationToken)
            => this.Wait(millisecondsTimeout, cancellationToken) ? new Guard(this) : null;
        public Guard? GuardedWait(TimeSpan timeout)
            => this.Wait(timeout) ? new Guard(this) : null;
        public Guard? GuardedWait(TimeSpan timeout, CancellationToken cancellationToken)
            => this.Wait(timeout, cancellationToken) ? new Guard(this) : null;

        public async Task<Guard?> GuardedWaitAsync(int millisecondsTimeout)
            => await this.WaitAsync(millisecondsTimeout) ? new Guard(this) : null;
        public async Task<Guard?> GuardedWaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
            => await this.WaitAsync(millisecondsTimeout, cancellationToken) ? new Guard(this) : null;
        public async Task<Guard?> GuardedWaitAsync(TimeSpan timeout)
            => await this.WaitAsync(timeout) ? new Guard(this) : null;
        public async Task<Guard?> GuardedWaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
            => await this.WaitAsync(timeout, cancellationToken) ? new Guard(this) : null;

        public sealed class Guard : IDisposable
        {
            private SemaphoreSlim _semaphore;
            private bool _disposed;

            public Guard(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
                _disposed = false;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _semaphore.Release();
                _disposed = true;
            }
        }
    }
}