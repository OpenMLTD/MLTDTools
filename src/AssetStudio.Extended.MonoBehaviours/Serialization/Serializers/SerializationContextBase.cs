using System;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    public abstract class SerializationContextBase : ISerializationContext {

        public abstract void RegisterConverter(ISimpleTypeConverter converter);

        public abstract void RegisterConverter(Type converterType);

        public void RegisterConverter<T>()
            where T : ISimpleTypeConverter {
            RegisterConverter(typeof(T));
        }

        public abstract ITypedSerializer GetSerializerOf(Type type);

    }
}
