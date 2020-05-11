using System;
using JetBrains.Annotations;
using OpenMLTD.ManifestTools.Web.TDAssets;

namespace OpenMLTD.ManifestTools.Extensions {
    internal static class UnityMobilePlatformExtensions {

        [NotNull]
        public static string ToStringFast(this UnityMobilePlatform platform) {
            switch (platform) {
                case UnityMobilePlatform.Unknown:
                    return "Unknown";
                case UnityMobilePlatform.Android:
                    return "Android";
                case UnityMobilePlatform.iOS:
                    return "iOS";
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }

    }
}
