using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    internal static class SerializingHelper {

        public static bool IsNumericType([NotNull] Type type) {
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong);
        }

        public static bool IsPrimitiveType([NotNull] Type type) {
            return IsNumericType(type) ||
                   type == typeof(char) || type == typeof(string);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type NonNullTypeOf([CanBeNull] object obj) {
            return obj?.GetType() ?? typeof(object);
        }

        [CanBeNull]
        public static object CreateDefaultOf([NotNull] Type type) {
            if (type.IsValueType) {
                // For structs we don't need to call its constructor
                return FormatterServices.GetSafeUninitializedObject(type);
            } else {
                return null;
            }
        }

        [CanBeNull]
        public static object ConvertValue([NotNull] ISimpleTypeConverter converter, [NotNull] Type serializedValueType, [NotNull] Type acceptedType, [CanBeNull] object value) {
            if (!converter.CanConvertFrom(serializedValueType) || !converter.CanConvertTo(acceptedType)) {
                throw new InvalidCastException($"Serialized type {serializedValueType} cannot be converted to {acceptedType}.");
            }

            return converter.ConvertTo(value, serializedValueType, acceptedType);
        }

    }
}
