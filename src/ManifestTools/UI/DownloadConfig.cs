using JetBrains.Annotations;
using OpenMLTD.ManifestTools.Web.TDAssets;

namespace OpenMLTD.ManifestTools.UI {
    public sealed class DownloadConfig {

        internal DownloadConfig([NotNull] string host, [NotNull] string unityVersion, int resourceVersion, bool isLatest, UnityMobilePlatform platform, [NotNull] string manifestAssetName) {
            Host = host;
            UnityVersion = unityVersion;
            ResourceVersion = resourceVersion;
            IsLatest = isLatest;
            Platform = platform;
            ManifestAssetName = manifestAssetName;
        }

        [NotNull]
        public string Host { get; }

        [NotNull]
        public string UnityVersion { get; }

        public int ResourceVersion { get; }

        public bool IsLatest { get; }

        public UnityMobilePlatform Platform { get; }

        [NotNull]
        public string ManifestAssetName { get; }

    }
}
