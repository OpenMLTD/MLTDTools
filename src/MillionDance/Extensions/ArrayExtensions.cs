using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item) {
            return Array.IndexOf(array, item);
        }

        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull] Predicate<T> predicate) {
            return Array.Find(array, predicate);
        }

        [NotNull, ItemCanBeNull]
        public static TResultItem[] SelectToArray<TItem, TResultItem>([NotNull, ItemCanBeNull] this TItem[] array, [NotNull] Func<TItem, TResultItem> map) {
            if (array.Length == 0) {
                return Array.Empty<TResultItem>();
            }

            var list = new List<TResultItem>();

            foreach (var item in array) {
                list.Add(map(item));
            }

            return list.ToArray();
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

        public static int Count<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull] Predicate<T> predicate) {
            var count = 0;

            foreach (var value in array) {
                if (predicate(value)) {
                    ++count;
                }
            }

            return count;
        }

        public static int Max<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull] Func<T, int> map) {
            if (array.Length == 0) {
                throw new ArgumentException("Array has no elements.");
            }

            int? result = null;

            foreach (var item in array) {
                var n = map(item);

                if (result.HasValue) {
                    if (result.Value < n) {
                        result = n;
                    }
                } else {
                    result = n;
                }
            }

            Debug.Assert(result != null, nameof(result) + " != null");

            return result.Value;
        }

        [Conditional("TRACE")]
        public static void AssertAllUnique<T>([NotNull, ItemNotNull] this T[] array) {
            var set = new HashSet<T>(array);
            Trace.Assert(array.Length == set.Count);
        }

    }
}
