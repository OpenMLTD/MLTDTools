using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MillionDanceView.Extensions {
    internal static class ArrayExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item) {
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < array.Length; ++i) {
                if (comparer.Equals(array[i], item)) {
                    return i;
                }
            }

            return -1;
        }

    }
}
