using System;

namespace Currycomb.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Populate<T>(this T[] source, T value)
        {
            Array.Fill(source, value);

            return source;
        }
    }
}