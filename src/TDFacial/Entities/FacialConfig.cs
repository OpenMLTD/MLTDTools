using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.MLTDTools.Applications.TDFacial.Entities {
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class FacialConfig {

        [JsonConstructor]
        public FacialConfig() {
            Version = LatestVersion;
            Expressions = new List<FacialExpression> {
                new FacialExpression()
            };
        }

        [JsonProperty]
        public int Version { get; set; }

        [JsonProperty]
        public List<FacialExpression> Expressions { get; set; }

        [NotNull]
        public static FacialConfig CreateKnown() {
            return new FacialConfig {
                Version = LatestVersion,
                Expressions = new List<FacialExpression> {
                    new FacialExpression {
                        Key = 0,
                        Description = "Default facial expression",
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
                        }
                    },
                    new FacialExpression {
                        Key = 1,
                        Description = "Very mild smile",
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
                        }
                    },
                    new FacialExpression {
                        Key = 3,
                        Description = "Staring far away",
                        Data = new Dictionary<string, float> {
                            ["M_egao"] = 0,
                            ["M_shinken"] = 0.32f,
                            ["M_wide"] = 0,
                            ["M_up"] = 0,
                            ["M_n2"] = 0,
                            ["M_down"] = 0,
                            ["M_odoroki"] = 0,
                            ["M_narrow"] = 0,
                            ["B_v_r"] = 0.12f,
                            ["B_v_l"] = 0.12f,
                            ["B_hati_r"] = 0.40f,
                            ["B_hati_l"] = 0.40f,
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
                        }
                    },
                    new FacialExpression {
                        Key = 5,
                        Description = "Happy",
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
                            ["E_wink_r"] = 1.0f,
                            ["E_wink_l"] = 1.0f,
                            ["E_open_r"] = 0,
                            ["E_open_l"] = 0,
                            ["EL_wide"] = 0,
                            ["EL_up"] = 0,
                        }
                    },
                    new FacialExpression {
                        Key = 22,
                        Description = "Right eye wink",
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
                            ["E_wink_r"] = 1,
                            ["E_wink_l"] = 0,
                            ["E_open_r"] = 0,
                            ["E_open_l"] = 0,
                            ["EL_wide"] = 0,
                            ["EL_up"] = 0,
                        }
                    },
                }
            };
        }

        private const int LatestVersion = 1;

    }
}
