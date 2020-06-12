using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    public abstract class TypedSerializerBase : ITypedSerializer {

        [NotNull]
        public virtual object DeserializeObject([NotNull] CustomType structure) {
            return DeserializeObject(structure, 0);
        }

        protected abstract object DeserializeObject(CustomType structure, int level);

        object ITypedSerializer.DeserializeObject(CustomType structure, int level) {
            return DeserializeObject(structure, level);
        }

    }
}
