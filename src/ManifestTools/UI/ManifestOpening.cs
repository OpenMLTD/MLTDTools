using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.UI {
    public sealed class ManifestOpening {

        private ManifestOpening(bool isLocal, [NotNull] string filePath, int version, bool isLatest) {
            IsLocal = isLocal;
            FilePath = filePath;
            Version = version;
            IsLatest = isLatest;
        }

        public bool IsLocal { get; }

        public bool IsLatest { get; }

        [NotNull]
        public string FilePath { get; }

        public int Version { get; }

        [NotNull]
        internal static ManifestOpening Local([NotNull] string filePath) {
            return new ManifestOpening(true, filePath, 0, false);
        }

        [NotNull]
        internal static ManifestOpening Remote(int version, bool isLatest) {
            return Remote(version, isLatest, null);
        }

        [NotNull]
        internal static ManifestOpening Remote(int version, bool isLatest, [CanBeNull] string filePath) {
            return new ManifestOpening(false, filePath ?? string.Empty, version, isLatest);
        }

    }
}
