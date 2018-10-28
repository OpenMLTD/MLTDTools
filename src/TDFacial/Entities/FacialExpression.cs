using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.MLTDTools.Applications.TDFacial.Entities {
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class FacialExpression : ICloneable {

        [JsonConstructor]
        public FacialExpression() {
            Description = "Default facial expression";
            Data = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0,
                ["B_v_l"] = 0,
                ["B_hati_r"] = 0,
                ["B_hati_l"] = 0,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 0,
                ["E_wink_l"] = 0,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            };
        }

        [JsonProperty]
        public int Key { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public Dictionary<string, float> Data { get; set; }

        [NotNull]
        public FacialExpression Clone() {
            var exp = new FacialExpression {
                Key = Key,
                Description = Description,
                Data = new Dictionary<string, float>()
            };

            foreach (var kv in Data) {
                exp.Data.Add(kv.Key, kv.Value);
            }

            return exp;
        }

        object ICloneable.Clone() {
            return Clone();
        }

    }
}
