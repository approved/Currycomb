using System;
using System.Threading;
using System.Threading.Tasks;

namespace Currycomb.Common.Extensions
{
    public static class ActionExtensions
    {
        // This extension method will execute an action after a specified delay.
        // If the action is called again before the specified delay, the delay will start over.
        public static Action<T> Debounce<T>(this Action<T> func, TimeSpan debounceTime)
        {
            CancellationTokenSource? cts = null;
            return arg =>
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                Task.Delay(debounceTime, cts.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                            func(arg);
                    }, TaskScheduler.Default);
            };
        }

        public static Action Debounce(this Action func, TimeSpan debounceTime)
        {
            CancellationTokenSource? cts = null;
            return () =>
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                Task.Delay(debounceTime, cts.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                            func();
                    }, TaskScheduler.Default);
            };
        }
    }
}
