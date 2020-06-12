using System;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    public interface ISerializationContext {

        void RegisterConverter([NotNull] ISimpleTypeConverter converter);

        void RegisterConverter([NotNull] Type converterType);

        [NotNull]
        ITypedSerializer GetSerializerOf([NotNull] Type type);

    }
}
