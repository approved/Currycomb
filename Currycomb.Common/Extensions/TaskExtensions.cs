using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Currycomb.Common.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Casts the result type of the input task as if it were covariant
        /// </summary>
        /// <typeparam name="T">The original result type of the task</typeparam>
        /// <typeparam name="TResult">The covariant type to return</typeparam>
        /// <param name="task">The target task to cast</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TResult> AsTask<T, TResult>(this Task<T> task)
            where T : TResult
            where TResult : class
            => await task;
    }
}