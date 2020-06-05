using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class ListExtensions {

        [Conditional("TRACE")]
        public static void AssertAllUnique<T>([NotNull, ItemNotNull] this List<T> list) {
            var set = new HashSet<T>(list);
            Trace.Assert(list.Count == set.Count);
        }

    }
}
