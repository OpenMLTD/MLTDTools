using System.Collections.Generic;
using JetBrains.Annotations;

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
        public static IEnumerable<(int Index, T Value)> Enumerate<T>([NotNull, ItemCanBeNull] this T[] e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);

                ++i;
            }
        }

        [NotNull]
        public static IEnumerable<(int Index, T Value)> Enumerate<T>([NotNull, ItemCanBeNull] this List<T> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);

                ++i;
            }
        }

        [NotNull]
        public static IEnumerable<(int Index, KeyValuePair<TKey, TValue> Value)> Enumerate<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);

                ++i;
            }
        }

    }
}
