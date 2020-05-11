using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.Web.Matsuri {
    internal static class MatsuriHimeApi {

        [NotNull]
        public static string LatestVersion() {
            const string url = BaseAddress + "/version/latest";
            return url;
        }

        [NotNull]
        public static string AssetVersions() {
            const string url = BaseAddress + "/version/assets";
            return url;
        }

        [NotNull]
        public static string AssetVersion(int version) {
            const string urlTemplate = BaseAddress + "/version/assets/{0}";
            return string.Format(urlTemplate, version.ToString());
        }

        private const string BaseAddress = "https://api.matsurihi.me/mltd/v1";

    }
}
