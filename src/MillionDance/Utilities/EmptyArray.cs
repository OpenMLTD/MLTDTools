using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class EmptyArray {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull, ItemNotNull]
        public static T[] Of<T>() {
            return EmptyArrayClass<T>.Value;
        }

        private static class EmptyArrayClass<T> {

            [NotNull, ItemNotNull]
            internal static readonly T[] Value = new T[0];

        }

    }
}
