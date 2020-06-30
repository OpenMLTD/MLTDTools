using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    internal interface ITypedSerializer {

        /// <summary>
        /// Deserialize a typed complex object.
        /// </summary>
        /// <param name="structure">Object value structure.</param>
        /// <param name="level">Current value level from the root. Root level is 0, which includes ScriptableObject's internal fields, and other user defined fields. Fields of the root fields is at level 1, etc.</param>
        /// <returns>Deserialized object instance.</returns>
        [CanBeNull]
        object DeserializeObject([NotNull] CustomType structure, int level);

    }
}
