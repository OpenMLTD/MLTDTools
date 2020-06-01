using JetBrains.Annotations;

namespace OpenMLTD.MiriTore.Database {
    public sealed class AssetInfo {

        public AssetInfo([NotNull] string resourceName, [NotNull] string contentHash, [NotNull] string remoteName, uint size) {
            ResourceName = resourceName;
            ContentHash = contentHash;
            RemoteName = remoteName;
            Size = size;
        }

        [NotNull]
        public string ResourceName { get; }

        [NotNull]
        public string ContentHash { get; }

        [NotNull]
        public string RemoteName { get; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public uint Size { get; }

    }
}
