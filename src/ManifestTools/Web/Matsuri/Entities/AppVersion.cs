using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.ManifestTools.Web.Matsuri.Entities {
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal sealed class AppVersion {

        [JsonConstructor]
        public AppVersion() {
        }

        [JsonProperty]
        public string Version { get; set; }

        // Format: 2020-05-10T12:00:00+09:00
        [JsonProperty]
        public string UpdateTime { get; set; }

        [JsonProperty]
        public int Revision { get; set; }

    }
}
