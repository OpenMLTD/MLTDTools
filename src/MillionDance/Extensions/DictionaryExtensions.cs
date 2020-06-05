using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class DictionaryExtensions {

        public static List<TResultItem> SelectManyToList<TKey, TValue, TResultItem>([NotNull] this Dictionary<TKey, TValue> dictionary, [NotNull] Func<KeyValuePair<TKey, TValue>, IEnumerable<TResultItem>> selector) {
            var result = new List<TResultItem>();

            foreach (var kv in dictionary) {
                var mapped = selector(kv);
                result.AddRange(mapped);
            }

            return result;
        }

        [Conditional("TRACE")]
        public static void AssertAllValuesUnique<TKey, TValue>([NotNull, ItemNotNull] this Dictionary<TKey, TValue> dictionary) {
            var set = new HashSet<TValue>(dictionary.Values);
            Trace.Assert(dictionary.Count == set.Count);
        }

    }
}
