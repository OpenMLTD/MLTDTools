using System;
using System.Collections.Generic;

namespace MillionDance.Utilities {
    internal static class ArrayCache {

        public static T[] Empty<T>() {
            var t = typeof(T);

            Array arr;

            if (EmptyArrays.ContainsKey(t)) {
                arr = EmptyArrays[t];
            } else {
                arr = new T[0];
                EmptyArrays[t] = arr;
            }

            return (T[])arr;
        }

        private static readonly Dictionary<Type, Array> EmptyArrays = new Dictionary<Type, Array>();

    }
}
