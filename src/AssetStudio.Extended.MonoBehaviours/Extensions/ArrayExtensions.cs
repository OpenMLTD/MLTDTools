using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Extensions {
    internal static class ArrayExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>([NotNull, ItemCanBeNull] this T[] array, [CanBeNull] T item) {
            return Array.IndexOf(array, item) >= 0;
        }

    }
}
