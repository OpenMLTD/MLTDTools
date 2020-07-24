using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class StringExtensions {

        // e.g. "abc/def/ghi",'/' -> "ghi"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string BreakLast([NotNull] this string str, char ch) {
            var index = str.LastIndexOf(ch);

            if (index < 0) {
                return str;
            }

            if (index == str.Length - 1) {
                throw new ArgumentException("The string must not end with the breaking character.");
            }

            return str.Substring(index + 1);
        }

        // e.g. "abc/def/ghi",'/' -> "abc/def"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string BreakUntilLast([NotNull] this string str, char ch) {
            var index = str.LastIndexOf(ch);

            if (index < 0) {
                return str;
            }

            return str.Substring(0, index);
        }

    }
}
