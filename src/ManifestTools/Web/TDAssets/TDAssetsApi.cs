using System;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.Web.TDAssets {
    // ReSharper disable once InconsistentNaming
    internal static class TDAssetsApi {

        [NotNull]
        public static string AssetUrl([NotNull] string host, [NotNull] string unityVersion, UnityMobilePlatform platform, int resourceVersion, [NotNull] string assetName) {
            var platformName = GetPlatformName(platform);
            var assetPath = $"/production/{unityVersion}/{platformName}/{assetName}";
            var assetUrl = $"https://{host}/{resourceVersion.ToString()}{assetPath}";
            return assetUrl;
        }

        [NotNull]
        private static string GetPlatformName(UnityMobilePlatform platform) {
            switch (platform) {
                case UnityMobilePlatform.Android:
                    return "Android";
                case UnityMobilePlatform.iOS:
                    return "iOS";
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }

        internal const string UnityVersion_Default = "2018";

        internal const string UnityVersion_Legacy = "2017.3";

        internal const string Host_CloundFront = "d2sf4w9bkv485c.cloudfront.net";

        internal const string Host_BN765 = "td-assets.bn765.com";

    }
}
