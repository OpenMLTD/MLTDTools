using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.ManifestTools.Web.Matsuri.Entities {
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal sealed class ResVersion {

        [JsonConstructor]
        public ResVersion() {
        }

        [JsonProperty]
        public int Version { get; set; }

        [JsonProperty]
        public string UpdateTime { get; set; }

        [JsonProperty]
        public string IndexName { get; set; }

    }
}
