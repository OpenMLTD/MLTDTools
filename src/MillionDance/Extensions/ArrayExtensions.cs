using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class ArrayExtensions {

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        public static bool ElementEquals<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull, ItemCanBeNull] T[] other) {
            if (ReferenceEquals(array, other)) {
                return true;
            }

            if (array == null && other == null) {
                return true;
            }

            if (array == null && other != null) {
                return false;
            }

            if (array != null && other == null) {
                return false;
            }

            if (array.Length != other.Length) {
                return false;
            }

            var len = array.Length;
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < len; ++i) {
                if (!comparer.Equals(array[i], other[i])) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex([NotNull] this int[] array, int item) {
            var i = 0;

            foreach (var n in array) {
                if (n == item) {
                    return i;
                }

                i += 1;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex([NotNull] this uint[] array, uint item) {
            var i = 0;

            foreach (var n in array) {
                if (n == item) {
                    return i;
                }

                i += 1;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item) {
            return FindIndex(array, item, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item, [CanBeNull] IEqualityComparer<T> comparer) {
            if (comparer == null) {
                comparer = EqualityComparer<T>.Default;
            }

            var i = 0;

            foreach (var obj in array) {
                if (comparer.Equals(obj, item)) {
                    return i;
                }

                i += 1;
            }

            return -1;
        }

    }
}
