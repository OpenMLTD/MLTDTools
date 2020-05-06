using System;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Extensions {
    internal static class TypeExtensions {

        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            var ifs = type.GetInterfaces();
            return ifs.Contains(interfaceType);
        }

        /// <example>
        /// var test = type.ImplementsGenericInterface(typeof(IDictionary&lt;,&gt;));
        /// </example>
        public static bool ImplementsGenericInterface([NotNull] this Type type, [NotNull] Type interfaceTypeDefinition) {
            var ifs = type.GetInterfaces();

            foreach (var @if in ifs) {
                if (@if.IsGenericType && @if.GetGenericTypeDefinition() == interfaceTypeDefinition) {
                    return true;
                }
            }

            return false;
        }

        [CanBeNull]
        public static Type GetGenericInterfaceImplementation([NotNull] this Type type, [NotNull] Type interfaceTypeDefinition) {
            var ifs = type.GetInterfaces();

            foreach (var @if in ifs) {
                if (@if.IsGenericType && @if.GetGenericTypeDefinition() == interfaceTypeDefinition) {
                    return @if;
                }
            }

            return null;
        }

    }
}
