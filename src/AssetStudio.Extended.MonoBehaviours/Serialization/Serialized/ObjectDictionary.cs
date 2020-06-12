using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serialized {
    public sealed class ObjectDictionary {

        internal ObjectDictionary([NotNull] Dictionary<object, object> dictionary, [NotNull] Type keyType, [NotNull] Type valueType) {
            Dictionary = dictionary;
            KeyType = keyType;
            ValueType = valueType;
        }

        [NotNull]
        public Dictionary<object, object> Dictionary { get; }

        /// <summary>
        /// Key type. Can be primitive types (including string), <see cref="CustomType"/> (typed object), <see cref="ObjectArray"/> (array), <see cref="ObjectDictionary"/> (dictionary),
        /// <see cref="RawData"/> (byte array), or <see cref="object"/> (unknown type, usually means the collection is empty).
        /// </summary>
        [NotNull]
        public Type KeyType { get; }

        /// <summary>
        /// Value type. Can be primitive types (including string), <see cref="CustomType"/> (typed object), <see cref="ObjectArray"/> (array), <see cref="ObjectDictionary"/> (dictionary),
        /// <see cref="RawData"/> (byte array), or <see cref="object"/> (unknown type, usually means the collection is empty).
        /// </summary>
        [NotNull]
        public Type ValueType { get; }

    }
}
