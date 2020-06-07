using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Extensions {
    internal static class ArrayExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item) {
            return Array.IndexOf(array, item) >= 0;
        }

        [NotNull, ItemCanBeNull]
        public static List<T> WhereToList<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull] Predicate<T> predicate) {
            var result = new List<T>();

            foreach (var item in array) {
                if (predicate(item)) {
                    result.Add(item);
                }
            }

            return result;
        }

        [NotNull, ItemCanBeNull]
        public static T[] WhereToArray<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull] Predicate<T> predicate) {
            if (array.Length == 0) {
                return Array.Empty<T>();
            }

            var list = WhereToList(array, predicate);

            return list.ToArray();
        }

    }
}
