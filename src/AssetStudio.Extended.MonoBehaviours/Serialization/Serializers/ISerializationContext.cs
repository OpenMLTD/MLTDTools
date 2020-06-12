using System;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Common;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    internal interface ISerializationContext {

        void RegisterConverter([NotNull] ISimpleTypeConverter converter);

        void RegisterConverter([NotNull] Type converterType);

        [NotNull]
        ITypedSerializer GetSerializerOf([NotNull] Type containerType);

        [NotNull]
        ConstructorManager Activator { get; }

        [NotNull]
        TypeConverterManager Converters { get; }

    }
}
