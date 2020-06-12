using System;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Common;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
    internal sealed class DynamicSerializationContext : SerializationContextBase {

        /// <summary>
        /// All information are loaded with reflection on demand, thus dynamic.
        /// </summary>
        public DynamicSerializationContext() {
            Activator = new ConstructorManager();
            Converters = new TypeConverterManager(this);
            Serializers = new TypedSerializerManager(this);
        }

        public override ConstructorManager Activator { get; }

        public override TypeConverterManager Converters { get; }

        [NotNull]
        public TypedSerializerManager Serializers { get; }

        public override ITypedSerializer GetSerializerOf(Type containerType) {
            return Serializers.GetSerializerOf(containerType);
        }

        public override void RegisterConverter(ISimpleTypeConverter converter) {
            Converters.RegisterConverter(converter);
        }

    }
}
