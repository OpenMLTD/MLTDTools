using System;
using System.Diagnostics;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.DefaultConverters {
    internal sealed class ByteToBooleanConverter : ISimpleTypeConverter {

        public bool CanConvertFrom(Type sourceType) {
            return sourceType == typeof(byte);
        }

        public bool CanConvertTo(Type destinationType) {
            return destinationType == typeof(bool);
        }

        public object ConvertTo(object value, Type sourceType, Type destinationType) {
            Debug.Assert(!ReferenceEquals(value, null));
            return ((byte)value) != 0;
        }

    }
}
