using System;
using JetBrains.Annotations;

namespace OpenMLTD.MiriTore.Extensions {
    public static class ArrayExtensions {

        public static int IndexOf<T>([NotNull] this T[] array, T value) {
            return Array.IndexOf(array, value);
        }

        public static bool Has<T>([NotNull] this T[] array, T value) {
            return IndexOf(array, value) >= 0;
        }

        public static T[] ReplaceInPlace<T>([NotNull] this T[] array, T find, T replace)
            where T : struct, IEquatable<T> {
            for (var i = 0; i < array.Length; ++i) {
                if (array[i].Equals(find)) {
                    array[i] = replace;
                }
            }

            return array;
        }

    }
}
