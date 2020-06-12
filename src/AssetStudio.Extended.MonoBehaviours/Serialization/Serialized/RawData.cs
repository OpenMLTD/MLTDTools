using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serialized {
    public sealed class RawData {

        internal RawData([NotNull] byte[] data) {
            Data = data;
        }

        [NotNull]
        public byte[] Data { get; }

    }
}
