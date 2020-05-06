using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class EnumerableExtensions {

        public static IEnumerable<(int Index, T Value)> Enumerate<T>([NotNull] this IEnumerable<T> e) {
            var i = 0;

            foreach (var item in e) {
                yield return (i, item);

                ++i;
            }
        }

    }
}