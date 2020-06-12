using System;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Common;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    internal abstract class SerializationContextBase : ISerializationContext {

        public abstract void RegisterConverter(ISimpleTypeConverter converter);

        public virtual void RegisterConverter(Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new InvalidCastException("The converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            var converter = Activator.CreateInstance(converterType, true);

            RegisterConverter((ISimpleTypeConverter)converter);
        }

        public void RegisterConverter<T>()
            where T : ISimpleTypeConverter {
            RegisterConverter(typeof(T));
        }

        public abstract ITypedSerializer GetSerializerOf(Type containerType);

        public abstract ConstructorManager Activator { get; }

        public abstract TypeConverterManager Converters { get; }

    }
}
