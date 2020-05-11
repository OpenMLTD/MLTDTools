using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.ManifestTools.Web.Matsuri.Entities {
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal sealed class LatestVersion {

        [JsonConstructor]
        public LatestVersion() {
        }

        [JsonProperty]
        public AppVersion App { get; set; }

        [JsonProperty]
        public ResVersion Res { get; set; }

    }
}
