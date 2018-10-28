using System.Collections.Generic;

namespace OpenMLTD.MLTDTools.Applications.TDFacial {
    internal static class Mappings {

        public static readonly IReadOnlyDictionary<string, string> Descriptions = new Dictionary<string, string> {
            ["M_egao"] = "ワ",
            ["M_shinken"] = "Λ",
            ["M_wide"] = "口大",
            ["M_up"] = "",
            ["M_n2"] = "",
            ["M_down"] = "",
            ["M_odoroki"] = "□",
            ["M_narrow"] = "口小",
            ["B_v_r"] = "",
            ["B_v_l"] = "",
            ["B_hati_r"] = "困る右",
            ["B_hati_l"] = "困る左",
            ["B_agari_r"] = "",
            ["B_agari_l"] = "",
            ["B_odoroki_r"] = "",
            ["B_odoroki_l"] = "",
            ["B_down"] = "下",
            ["B_yori"] = "",
            ["E_metoji_r"] = "FOR INTERNAL USE. KEEP IT 0.",
            ["E_metoji_l"] = "FOR INTERNAL USE. KEEP IT 0.",
            ["E_wink_r"] = "ウィンク右",
            ["E_wink_l"] = "ウィンク",
            ["E_open_r"] = "見開く右",
            ["E_open_l"] = "見開く左",
            ["EL_wide"] = "",
            ["EL_up"] = "",
        };

    }
}
