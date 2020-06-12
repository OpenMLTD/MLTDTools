using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    internal static class SerializationContextCreator {

        [NotNull]
        public static ISerializationContext Create() {
            return CreateDynamic();
        }

        [NotNull]
        public static ISerializationContext CreateDynamic() {
            return new DynamicSerializationContext();
        }

    }
}
