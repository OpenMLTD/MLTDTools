using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Extensions.Internal;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class EnumerableExtensions {

        [NotNull]
        public static IEnumerable<(int Index, T Value)> Enumerate<T>([NotNull, ItemCanBeNull] this IEnumerable<T> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);

                ++i;
            }
        }

        [NotNull]
        public static ArrayEnumerateHelper<T> Enumerate<T>([NotNull, ItemCanBeNull] this T[] e) {
            return new ArrayEnumerateHelper<T>(e);
        }

        [NotNull]
        public static (int Index, T Value)[] EnumerateAll<T>([NotNull, ItemCanBeNull] this T[] e) {
            var itemCount = e.Length;

            if (itemCount == 0) {
                return Array.Empty<(int, T)>();
            }

            var result = new (int, T)[itemCount];

            for (var i = 0; i < itemCount; i += 1) {
                result[i] = (i, e[i]);
            }

            return result;
        }

        [NotNull]
        public static IEnumerable<(int Index, T Item)> Enumerate<T>([NotNull, ItemCanBeNull] this List<T> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);
                ++i;
            }
        }

        [NotNull]
        public static (int Index, T Value)[] EnumerateAll<T>([NotNull, ItemCanBeNull] this List<T> e) {
            var itemCount = e.Count;

            if (itemCount == 0) {
                return Array.Empty<(int, T)>();
            }

            var result = new (int, T)[itemCount];

            for (var i = 0; i < itemCount; i += 1) {
                result[i] = (i, e[i]);
            }

            return result;
        }

        [NotNull]
        public static IEnumerable<(int Index, KeyValuePair<TKey, TValue> Item)> Enumerate<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);
                ++i;
            }
        }

    }
}
