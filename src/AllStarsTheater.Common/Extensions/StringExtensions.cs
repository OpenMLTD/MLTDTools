using System;
using JetBrains.Annotations;

namespace OpenMLTD.AllStarsTheater.Extensions {
    public static class StringExtensions {

        public static bool Contains([NotNull] this string @this, [NotNull] string str, StringComparison comparison) {
            return @this.IndexOf(str, comparison) >= 0;
        }

    }
}
