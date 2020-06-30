using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    public abstract class TypedSerializerBase : ITypedSerializer {

        [CanBeNull]
        public virtual object DeserializeObject([NotNull] CustomType structure) {
            return DeserializeObject(structure, 0);
        }

        [CanBeNull]
        protected abstract object DeserializeObject([NotNull] CustomType structure, int level);

        object ITypedSerializer.DeserializeObject(CustomType structure, int level) {
            return DeserializeObject(structure, level);
        }

    }
}
