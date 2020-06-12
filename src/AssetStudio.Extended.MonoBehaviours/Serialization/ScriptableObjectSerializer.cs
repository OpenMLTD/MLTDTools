using System;
using System.Diagnostics;
using AssetStudio.Extended.MonoBehaviours.Serialization.DefaultConverters;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    public sealed class ScriptableObjectSerializer {

        public ScriptableObjectSerializer() {
            _context = new Lazy<ISerializationContext>(SerializationContextCreator.Create);

            // In old versions(?) Unity serializes booleans as bytes
            WithConverter<ByteToBooleanConverter>();
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter<T>()
            where T : ISimpleTypeConverter {
            return WithConverter(typeof(T));
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter([NotNull] Type converterType) {
            _context.Value.RegisterConverter(converterType);
            return this;
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter([NotNull] ISimpleTypeConverter converter) {
            _context.Value.RegisterConverter(converter);
            return this;
        }

        [CanBeNull]
        public T Deserialize<T>([NotNull] MonoBehaviour monoBehavior) where T : new() {
            var ret = Deserialize(monoBehavior, typeof(T));
            return (T)ret;
        }

        [CanBeNull]
        public object Deserialize([NotNull] MonoBehaviour monoBehavior, [NotNull] Type type) {
            if (monoBehavior == null) {
                throw new ArgumentNullException(nameof(monoBehavior));
            }

            var typeNodes = monoBehavior.serializedType.m_Nodes;

            // Types nodes are empty, which means null
            if (typeNodes == null) {
                return SerializingHelper.CreateDefaultOf(type);
            }

            // We should actually read the contents and deserialize an object
            var structure = StructureReader.ReadMembers(typeNodes, monoBehavior.reader);

            Debug.Assert(structure.Length == 1);

            var rootObject = structure[0].Value;

            Debug.Assert(rootObject != null);
            Debug.Assert(rootObject is CustomType);

            var serializer = _context.Value.GetSerializerOf(type);

            var obj = serializer.DeserializeObject((CustomType)rootObject, 0);

            return obj;
        }

        [NotNull]
        private readonly Lazy<ISerializationContext> _context;

    }
}
