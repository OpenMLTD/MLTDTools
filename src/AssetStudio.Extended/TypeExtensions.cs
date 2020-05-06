using System;
using System.Linq;
using JetBrains.Annotations;

namespace AssetStudio.Extended {
    internal static class TypeExtensions {

        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            var ifs = type.GetInterfaces();
            return ifs.Contains(interfaceType);
        }

        public static bool ImplementsGenericInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            var ifs = type.GetInterfaces();
            return ifs.Any(@if => @if.IsGenericType && @if.GetGenericTypeDefinition() == interfaceType);
        }

    }
}