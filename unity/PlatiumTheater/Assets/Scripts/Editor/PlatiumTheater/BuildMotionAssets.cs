using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PlatiumTheater.Internal;
using UnityEditor;

namespace PlatiumTheater {
    public static class BuildMotionAssets {

        [MenuItem("Tools/MLTD Tools/Build Motion Assets/Dancing")]
        public static void Build() {
            const string dancingAssetsInputRoot = "Assets/Imas/Resources/Exclude/Imo/$Dance";
            const string dancingAssetsOutputRoot = "Assets/Imas/Resources/Exclude/Imo/Dance";

            if (!Directory.Exists(dancingAssetsInputRoot)) {
                return;
            }

            var subDirs = Directory.GetDirectories(dancingAssetsInputRoot);

            foreach (var subDir in subDirs) {
                var vmdFiles = Directory.GetFiles(subDir, "*.vmd");

                if (vmdFiles.Length == 0) {
                    continue;
                }

                var dir = new DirectoryInfo(subDir);

                BuildVmd(dancingAssetsOutputRoot, dir.Name, vmdFiles[0]);
            }
        }

        private static void BuildVmd([NotNull] string outputRoot, [NotNull] string subDirName, [NotNull] string vmdFilePath) {
            var vmd = VmdReader.ReadMotionFrom(vmdFilePath);

            var set = new HashSet<string>();

            foreach (var frame in vmd.BoneFrames) {
                set.Add(frame.Name);
            }

            foreach (var name in set) {
                UnityEngine.Debug.Log(name);
            }
        }

        private static readonly Dictionary<string, string> VmdJointNameMap = new Dictionary<string, string> {
            ["全ての親"] = "POSITION",
            ["センター"] = "MODEL_00",
            ["腰"] = "MODEL_00/BODY_SCALE/BASE",
            ["上半身"] = "MODEL_00/BODY_SCALE/BASE/MUNE1",
            ["上半身2"] = "MODEL_00/BODY_SCALE/BASE/MUNE2",
            ["左肩P"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L",
            ["左肩"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L",
            ["左ひじ"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L",
            ["左手首"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L",
            ["左親指０"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L",
            ["左親指１"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L",
            ["左親指２"] = "MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L",
        };

    }
}
