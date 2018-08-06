using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Utilities {
    internal static class MorphUtils {

        [NotNull]
        public static string LookupMorphName([NotNull] string mltdMorphName) {
            if (MorphNameMap.ContainsKey(mltdMorphName)) {
                return MorphNameMap[mltdMorphName];
            } else {
                return mltdMorphName;
            }
        }

        private static readonly IReadOnlyDictionary<string, string> MorphNameMap = new Dictionary<string, string> {
            ["blendShape1.M_a"] = "あ",
            ["blendShape1.M_i"] = "い",
            ["blendShape1.M_u"] = "う",
            ["blendShape1.M_e"] = "え",
            ["blendShape1.M_o"] = "お",
            ["blendShape1.M_n"] = "ん",
            ["blendShape1.M_egao"] = "ワ", // TDA
            ["blendShape1.M_shinken"] = "Λ",
            ["blendShape1.M_wide"] = "口大",
            //["blendShape1.M_up"] = "",
            ["blendShape1.M_n2"] = "Λ",
            //["blendShape1.M_down"] = "",
            ["blendShape1.M_odoroki"] = "□", // TDA
            ["blendShape1.M_narrow"] = "口小",
            //["blendShape1.B_v_r"] = "",
            //["blendShape1.B_v_l"] = "",
            ["blendShape1.B_hati_r"] = "困る右",
            ["blendShape1.B_hati_l"] = "困る左",
            //["blendShape1.B_agari_r"] = "",
            //["blendShape1.B_agari_l"] = "",
            //["blendShape1.B_odoroki_r"] = "",
            //["blendShape1.B_odoroki_l"] = "",
            ["blendShape1.B_down"] = "下",
            //["blendShape1.B_yori"] = "",
            ["blendShape1.E_metoji_r"] = "ｳｨﾝｸ２右", // TDA; note that the morph's name in TDA style models IS half-width (not full-width like the other morphs)
            ["blendShape1.E_metoji_l"] = "ウィンク２", // TDA
            ["blendShape1.E_wink_r"] = "ウィンク右", // TDA
            ["blendShape1.E_wink_l"] = "ウィンク", // TDA
            ["blendShape1.E_open_r"] = "見開く右",
            ["blendShape1.E_open_l"] = "見開く左",
            //["blendShape1.EL_wide"] = "",
            //["blendShape1.EL_up"] = "",

            ["blendShape1.E_metoji"] = "まばたき",
        };

    }
}
