using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class MorphUtils {

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string LookupFullMorphName([NotNull] string mltdMorphName) {
            var truncMorphName = mltdMorphName.Substring(12); // - "blendShape1."
            return LookupMorphName(truncMorphName);
        }

        [NotNull]
        public static string LookupMorphName([NotNull] string mltdTruncMorphName) {
            if (MorphNameMap.ContainsKey(mltdTruncMorphName)) {
                return MorphNameMap[mltdTruncMorphName];
            } else {
                return mltdTruncMorphName;
            }
        }

        private static readonly IReadOnlyDictionary<string, string> MorphNameMap = new Dictionary<string, string> {
            ["M_a"] = "あ",
            ["M_i"] = "い",
            ["M_u"] = "う",
            ["M_e"] = "え",
            ["M_o"] = "お",
            ["M_n"] = "ん",
            ["M_egao"] = "ワ", // TDA
            ["M_shinken"] = "Λ",
            ["M_wide"] = "口大",
            //["M_up"] = "",
            //["M_n2"] = "Λ",
            //["M_down"] = "",
            ["M_odoroki"] = "□", // TDA
            ["M_narrow"] = "口小",
            //["B_v_r"] = "",
            //["B_v_l"] = "",
            ["B_hati_r"] = "困る右",
            ["B_hati_l"] = "困る左",
            //["B_agari_r"] = "",
            //["B_agari_l"] = "",
            //["B_odoroki_r"] = "",
            //["B_odoroki_l"] = "",
            ["B_down"] = "下",
            //["B_yori"] = "",
            ["E_metoji_r"] = "ｳｨﾝｸ２右", // TDA; note that the morph's name in TDA style models IS half-width (not full-width like the other morphs)
            ["E_metoji_l"] = "ウィンク２", // TDA
            ["E_wink_r"] = "ウィンク右", // TDA
            ["E_wink_l"] = "ウィンク", // TDA
            ["E_open_r"] = "見開く右",
            ["E_open_l"] = "見開く左",
            //["EL_wide"] = "",
            //["EL_up"] = "",

            ["E_metoji"] = "まばたき",
        };

    }
}
