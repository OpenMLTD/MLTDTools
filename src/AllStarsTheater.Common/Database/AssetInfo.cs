using JetBrains.Annotations;

namespace OpenMLTD.AllStarsTheater.Database {
    public sealed class AssetInfo {

        public AssetInfo([NotNull] string resourceName, [NotNull] string contentHash, [NotNull] string remoteName, uint size) {
            ResourceName = resourceName;
            ContentHash = contentHash;
            RemoteName = remoteName;
            Size = size;
            NameEndingByte = 0x93;
        }

        internal AssetInfo([NotNull] string resourceName, [NotNull] string contentHash, [NotNull] string remoteName, uint size, byte nameEndingByte) {
            ResourceName = resourceName;
            ContentHash = contentHash;
            RemoteName = remoteName;
            Size = size;
            NameEndingByte = nameEndingByte;
        }

        [NotNull]
        public string ResourceName { get; }

        [NotNull]
        public string ContentHash { get; set; }

        [NotNull]
        public string RemoteName { get; set; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Should this use a normal size length (uint16, 0xcd) or a long size length (uint32, 0xce)?
        /// </summary>
        public bool IsLongSize => Size > 65535u;

        /// <summary>
        /// I don't know what this is, but it seems to be 0x93.
        /// </summary>
        public byte NameEndingByte { get; }

    }
}
