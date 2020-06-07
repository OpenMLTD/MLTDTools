using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.DefaultConverters;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Managing {
    internal sealed class SerializerManager {

        public SerializerManager() {
            _serializers = new ConditionalWeakTable<Type, TypedSerializerBase>();

            _createdTypeConverters = new Dictionary<Type, ISimpleTypeConverter>(10);

            // In old versions(?) Unity serializes booleans as bytes
            RegisterConverter<ByteToBooleanConverter>();
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
        public ISimpleTypeConverter GetConverterOf([NotNull] Type converterType) {
            _createdTypeConverters.TryGetValue(converterType, out var result);
            return result;
        }

        [NotNull]
        public ISimpleTypeConverter GetOrRegisterConverter([NotNull] Type converterType) {
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
        public object TryConvertValueType([NotNull] Type serializedValueType, [NotNull] Type acceptedType, [CanBeNull] object value, [CanBeNull] Type converterTypeHint) {
            if (acceptedType == serializedValueType) {
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

        [CanBeNull]
        public ISimpleTypeConverter FindConverter([NotNull] Type sourceType, [NotNull] Type destinationType, [CanBeNull] Type converterTypeHint) {
            ISimpleTypeConverter converter;

            foreach (var kv in _createdTypeConverters) {
                converter = kv.Value;

                if (converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType)) {
                    return converter;
                }
            }

            if (converterTypeHint == null) {
                return null;
            }

            converter = GetOrRegisterConverter(converterTypeHint);

            if (!(converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType))) {
                return null;
            }

            return converter;
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
