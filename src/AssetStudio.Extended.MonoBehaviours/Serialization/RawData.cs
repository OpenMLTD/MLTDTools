using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    internal sealed class RawData {

        public RawData([NotNull] byte[] data) {
            Data = data;
        }

        [NotNull]
        public byte[] Data { get; }

    }
}
