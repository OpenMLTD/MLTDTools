using System;
using JetBrains.Annotations;

namespace OpenMLTD.MiriTore.Database {
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
        public string ContentHash { get; }

        [NotNull]
        public string RemoteName { get; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// Should this use a normal size length (uint16, 0xcd) or a long size length (uint32, 0xce)?
        /// </summary>
        public bool IsLongSize => Size > 65535u;

        /// <summary>
        /// I don't know what this is, but it seems to be 0x93.
        /// </summary>
        public byte NameEndingByte { get; }

        public override int GetHashCode() {
            const int seed = 17;
            const int mul = 31;

            var hash = seed;
            hash = hash + ResourceName.GetHashCode() * mul;
            hash = hash + RemoteName.GetHashCode() * mul;
            hash = hash + ContentHash.GetHashCode() * mul;
            hash = hash + Size.GetHashCode() * mul;

            return hash;
        }

        public static bool operator ==([CanBeNull] AssetInfo x, [CanBeNull] AssetInfo y) {
            if (ReferenceEquals(x, null)) {
                if (ReferenceEquals(y, null)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                if (ReferenceEquals(y, null)) {
                    return false;
                } else {
                    return string.Equals(x.ResourceName, y.ResourceName, StringComparison.Ordinal) &&
                           string.Equals(x.RemoteName, y.ResourceName, StringComparison.Ordinal) &&
                           string.Equals(x.ContentHash, y.ContentHash, StringComparison.Ordinal) &&
                           x.Size == y.Size;
                }
            }
        }

        public static bool operator !=([CanBeNull] AssetInfo x, [CanBeNull] AssetInfo y) {
            return !(x == y);
        }

    }
}
