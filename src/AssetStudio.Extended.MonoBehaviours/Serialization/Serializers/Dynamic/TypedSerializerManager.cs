using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
    internal sealed class TypedSerializerManager {

        public TypedSerializerManager([NotNull] DynamicSerializationContext context) {
            Context = context;
            _serializers = new ConditionalWeakTable<Type, ITypedSerializer>();
        }

        [NotNull]
        public DynamicSerializationContext Context { get; }

        [NotNull]
        public ITypedSerializer GetSerializerOf([NotNull] Type type) {
            if (SerializingHelper.IsPrimitiveType(type)) {
                throw new ArgumentException("Cannot obtain a serializer for primitive types.");
            }

            return _serializers.GetValue(type, CreateTypedSerializer);
        }

        [NotNull]
        private ITypedSerializer CreateTypedSerializer([NotNull] Type targetType) {
            var serializer = new TypedSerializer(Context, targetType);
            return serializer;
        }

        [NotNull]
        private readonly ConditionalWeakTable<Type, ITypedSerializer> _serializers;

    }
}
