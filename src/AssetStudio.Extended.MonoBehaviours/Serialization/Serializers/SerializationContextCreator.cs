using AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Static;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers {
    internal static class SerializationContextCreator {

        [NotNull]
        public static ISerializationContext CreateStatic() {
            return new StaticSerializationContext();
        }

    }
}
