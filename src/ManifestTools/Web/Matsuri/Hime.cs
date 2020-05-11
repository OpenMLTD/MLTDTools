using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.ManifestTools.Web.Matsuri.Entities;

namespace OpenMLTD.ManifestTools.Web.Matsuri {
    internal sealed class Hime {

        static Hime() {
            Serializer = new JsonSerializer();
        }

        [NotNull]
        public static async Task<LatestVersion> GetLatestVersion() {
            var url = MatsuriHimeApi.LatestVersion();
            var text = await WebCommon.HttpClient.GetStringAsync(url);

            var result = DeserializeString<LatestVersion>(text);

            return result;
        }

        [NotNull]
        public static async Task<ResVersion[]> GetAssetVersions() {
            var url = MatsuriHimeApi.AssetVersions();
            var text = await WebCommon.HttpClient.GetStringAsync(url);

            var result = DeserializeString<ResVersion[]>(text);

            return result;
        }

        [NotNull]
        public static async Task<ResVersion> GetAssetVersion(int version) {
            var url = MatsuriHimeApi.AssetVersion(version);
            var text = await WebCommon.HttpClient.GetStringAsync(url);

            var result = DeserializeString<ResVersion>(text);

            return result;
        }

        private static T DeserializeString<T>([NotNull] string s) {
            T result;

            using (var stringReader = new StringReader(s)) {
                using (var jsonReader = new JsonTextReader(stringReader)) {
                    result = Serializer.Deserialize<T>(jsonReader);
                }
            }

            return result;
        }

        [NotNull]
        private static readonly JsonSerializer Serializer;

    }
}
