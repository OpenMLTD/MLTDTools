using System;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Static {
    internal sealed class StaticSerializationContext : SerializationContextBase {

        public StaticSerializationContext() {
            Activator = new ConstructorManager();
            Converters = new TypeConverterManager(this);
            Serializers = new TypedSerializerManager(this);
        }

        [NotNull]
        public ConstructorManager Activator { get; }

        [NotNull]
        public TypeConverterManager Converters { get; }

        [NotNull]
        public TypedSerializerManager Serializers { get; }

        public override ITypedSerializer GetSerializerOf(Type type) {
            return Serializers.GetSerializerOf(type);
        }

        public override void RegisterConverter(ISimpleTypeConverter converter) {
            Converters.RegisterConverter(converter);
        }

        public override void RegisterConverter(Type converterType) {
            Converters.RegisterConverter(converterType);
        }

    }
}
