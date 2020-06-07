using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Managing {
    internal sealed class SerializerManager {

        public SerializerManager() {
            _serializers = new ConditionalWeakTable<Type, TypedSerializerBase>();
            _createdTypeConverters = new Dictionary<Type, ISimpleTypeConverter>(10);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypedSerializerBase GetSerializerOf<T>() {
            return GetSerializerOf(typeof(T));
        }

        [NotNull]
        public TypedSerializerBase GetSerializerOf([NotNull] Type type) {
            if (SerializingHelper.IsPrimitiveType(type)) {
                throw new ArgumentException("Cannot obtain a serializer for primitive types.");
            }

            return _serializers.GetValue(type, CreateTypedSerializer);
        }

        public void RegisterConverter<T>()
            where T : ISimpleTypeConverter {
            RegisterConverter(typeof(T));
        }

        public void RegisterConverter([NotNull] Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new InvalidCastException("The converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            var converter = Activator.CreateInstance(converterType, true);

            RegisterConverter((ISimpleTypeConverter)converter);
        }

        public void RegisterConverter([NotNull] ISimpleTypeConverter converter) {
            var converterType = converter.GetType();
            _createdTypeConverters[converterType] = converter;
        }

        [CanBeNull]
        public object TryConvertTypeOfValue([NotNull] Type serializedValueType, [NotNull] Type acceptedType, [CanBeNull] object value, [CanBeNull] Type converterTypeHint) {
            if (acceptedType.IsAssignableFrom(serializedValueType)) {
                return value;
            }

            var converter = FindConverter(serializedValueType, acceptedType, converterTypeHint);

            if (converter != null) {
                var convertedValue = SerializingHelper.ConvertValue(converter, serializedValueType, acceptedType, value);

                return convertedValue;
            } else {
                return value;
            }
        }

        [NotNull]
        private ISimpleTypeConverter GetOrRegisterConverter([NotNull] Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new ArgumentException("Converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            ISimpleTypeConverter converter;

            if (_createdTypeConverters.ContainsKey(converterType)) {
                converter = _createdTypeConverters[converterType];
            } else {
                converter = (ISimpleTypeConverter)Activator.CreateInstance(converterType, true);
                _createdTypeConverters[converterType] = converter;
            }

            return converter;
        }

        [CanBeNull]
        private ISimpleTypeConverter FindConverter([NotNull] Type sourceType, [NotNull] Type destinationType, [CanBeNull] Type converterTypeHint) {
            ISimpleTypeConverter converter;

            // Try with hinted converter type first
            if (converterTypeHint != null) {
                converter = GetOrRegisterConverter(converterTypeHint);

                if (converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType)) {
                    return converter;
                }
            }

            // If the type hint failed, search all registered converters
            foreach (var kv in _createdTypeConverters) {
                converter = kv.Value;

                if (converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType)) {
                    return converter;
                }
            }

            return null;
        }

        [NotNull]
        private TypedSerializerBase CreateTypedSerializer([NotNull] Type targetType) {
            var serializer = new TypedSerializerBase(this, targetType);
            return serializer;
        }

        [NotNull]
        private readonly ConditionalWeakTable<Type, TypedSerializerBase> _serializers;

        [NotNull]
        private readonly Dictionary<Type, ISimpleTypeConverter> _createdTypeConverters;

    }
}
