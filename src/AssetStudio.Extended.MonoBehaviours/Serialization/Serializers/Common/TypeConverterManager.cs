using System;
using System.Collections.Generic;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Common {
    internal sealed class TypeConverterManager {

        public TypeConverterManager([NotNull] ISerializationContext context) {
            Context = context;
            _createdTypeConverters = new Dictionary<Type, ISimpleTypeConverter>(10);
        }

        [NotNull]
        public ISerializationContext Context { get; }

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
        private ISimpleTypeConverter GetOrRegisterConverter([NotNull] Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new ArgumentException("Converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            ISimpleTypeConverter converter;

            if (_createdTypeConverters.ContainsKey(converterType)) {
                converter = _createdTypeConverters[converterType];
            } else {
                converter = (ISimpleTypeConverter)Context.Activator.CreateInstance(converterType, true);
                _createdTypeConverters[converterType] = converter;
            }

            return converter;
        }

        [NotNull]
        private readonly Dictionary<Type, ISimpleTypeConverter> _createdTypeConverters;

    }
}
