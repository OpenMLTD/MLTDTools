using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.DefaultConverters;
using AssetStudio.Extended.MonoBehaviours.Serialization.Managing;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;
using PropOrField = AssetStudio.Extended.MonoBehaviours.Serialization.Managing.MemberSetter;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    public sealed class ScriptableObjectSerializer {

        public ScriptableObjectSerializer() {
            _manager = new SerializerManager();
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter<T>()
            where T : ISimpleTypeConverter {
            _manager.RegisterConverter<T>();
            return this;
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter([NotNull] Type converterType) {
            _manager.RegisterConverter(converterType);
            return this;
        }

        [NotNull]
        public ScriptableObjectSerializer WithConverter([NotNull] ISimpleTypeConverter converter) {
            _manager.RegisterConverter(converter);
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

            Debug.Assert(structure.Count == 1);

            var structureValues = structure.Values.ToArray();
            var rootObject = structureValues[0];

            Debug.Assert(rootObject != null);
            Debug.Assert(rootObject is CustomType);

            var serializer = _manager.GetSerializerOf(type);

            var obj = serializer.DeserializeObject((CustomType)rootObject);

            return obj;
        }

        [NotNull]
        private readonly SerializerManager _manager;

    }
}
