using Newtonsoft.Json;

namespace OpenMLTD.ThankYou {
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class DnsConfig {

        [JsonConstructor]
        internal DnsConfig() {
        }

        [JsonProperty(PropertyName = "local_ip")]
        public string LocalIP { get; private set; }

        [JsonProperty(PropertyName = "redirect")]
        public string[] RedirectPatterns { get; private set; }

        [JsonProperty(PropertyName = "upstream_dns")]
        public string UpstreamDns { get; private set; }

    }
}