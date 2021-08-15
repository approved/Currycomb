using System;
using System.Collections.Generic;
using System.Linq;

namespace Currycomb.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> If<T>(this IEnumerable<T> source, bool predicate, Func<IEnumerable<T>, IEnumerable<T>> func)
            => predicate ? func(source) : source;

        public static IEnumerable<T> Inspect<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> Once<T>(this IEnumerable<T> source, Action action)
        {
            if (source.Any())
                action();

            foreach (var item in source)
                yield return item;
        }
    }
}
