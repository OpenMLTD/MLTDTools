using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Unity;
using MillionDance.Entities.Vmd;
using OpenTK;

namespace MillionDance {
    public static class VmdCreator {

        [NotNull]
        public static VmdMotion FromDanceData([NotNull] CharacterImasMotionAsset dance) {
            const string modelName = "MODEL_00";

            var boneFrames = new List<VmdBoneFrame>();

            var anim = Animation.CreateFrom(dance);

            //var maxx = anim.KeyFrames.Max(f => f.AngleX ?? 0);
            //var maxy = anim.KeyFrames.Max(f => f.AngleY ?? 0);
            //var maxz = anim.KeyFrames.Max(f => f.AngleZ ?? 0);
            //var minx = anim.KeyFrames.Min(f => f.AngleX ?? 0);
            //var miny = anim.KeyFrames.Min(f => f.AngleY ?? 0);
            //var minz = anim.KeyFrames.Min(f => f.AngleZ ?? 0);

            var counter = 0;

            foreach (var keyFrame in anim.KeyFrames) {
                // MLTD 60 fps -> VMD 30 fps
                ++counter;

                if (counter % 2 == 0) {
                    continue;
                }

                var boneName = BoneNameMap.ContainsKey(keyFrame.Path) ? BoneNameMap[keyFrame.Path] : "UNDEFINED";

                var boneFrame = new VmdBoneFrame(keyFrame.FrameIndex / 2, boneName);

                if (keyFrame.HasPositions) {
                    Debug.Assert(keyFrame.PositionX != null, "keyFrame.PositionX != null");
                    var x = keyFrame.PositionX.Value;
                    Debug.Assert(keyFrame.PositionY != null, "keyFrame.PositionY != null");
                    var y = keyFrame.PositionY.Value;
                    Debug.Assert(keyFrame.PositionZ != null, "keyFrame.PositionZ != null");
                    var z = keyFrame.PositionZ.Value;
                    boneFrame.Position = new Vector3(x, y, z);
                }

                // TODO: The coordinate system of MLTD and VMD?
                // TODO: The interpretation of frame values (esp. rotation angles)?
                if (keyFrame.HasRotations) {
                    Debug.Assert(keyFrame.AngleX != null, "keyFrame.AngleX != null");
                    var ax = MathHelper.DegreesToRadians(keyFrame.AngleX.Value);
                    //var ax = keyFrame.AngleX.Value / 100;
                    //var ax = MathHelper.DegreesToRadians(keyFrame.AngleX.Value / 1.5f);
                    Debug.Assert(keyFrame.AngleY != null, "keyFrame.AngleY != null");
                    var ay = MathHelper.DegreesToRadians(keyFrame.AngleY.Value);
                    //var ay = keyFrame.AngleY.Value / 100;
                    //var ay = MathHelper.DegreesToRadians(keyFrame.AngleY.Value / 1.5f);
                    Debug.Assert(keyFrame.AngleZ != null, "keyFrame.AngleZ != null");
                    var az = MathHelper.DegreesToRadians(keyFrame.AngleZ.Value);
                    //var az = keyFrame.AngleZ.Value / 100;
                    //var az = MathHelper.DegreesToRadians(keyFrame.AngleZ.Value / 1.5f);

                    boneFrame.Rotation = Quaternion.FromEulerAngles(ax, ay, az);
                }

                boneFrames.Add(boneFrame);
            }

            var facialFrames = new VmdFacialFrame[0];
            var cameraFrames = new VmdCameraFrame[0];
            var lightFrames = new VmdLightFrame[0];

            return new VmdMotion(modelName, boneFrames.ToArray(), facialFrames, cameraFrames, lightFrames, null);
        }

        private static readonly IReadOnlyDictionary<string, string> BoneNameMap = new Dictionary<string, string> {
            ["POSITION"] = "操作中心",
            ["POSITION/SCALE_POINT"] = "全ての親",
            ["MODEL_00"] = "センター",
            ["MODEL_00/BODY_SCALE/BASE"] = "グルーブ",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI"] = "腰",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L"] = "左足",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L"] = "左ひざ",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L"] = "左足首",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L"] = "左つま先",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R"] = "右足",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R"] = "右ひざ",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R"] = "右足首",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R"] = "右つま先",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1"] = "上半身",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2"] = "上半身2",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI"] = "首",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "頭",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L"] = "左肩",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L"] = "左腕",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L"] = "左ひじ",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L"] = "左手首",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L"] = "左人指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L"] = "左人指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L"] = "左人指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L"] = "左ダミー",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L"] = "左小指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L"] = "左小指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L"] = "左小指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L"] = "左薬指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L"] = "左薬指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L"] = "左薬指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L"] = "左中指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L"] = "左中指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L"] = "左中指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L"] = "左親指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L"] = "左親指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L"] = "左親指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R"] = "右肩",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R"] = "右腕",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R"] = "右ひじ",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R"] = "右手首",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R"] = "右人指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R"] = "右人指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R"] = "右人指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R"] = "右ダミー",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R"] = "右小指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R"] = "右小指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R"] = "右小指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R"] = "右薬指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R"] = "右薬指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R"] = "右薬指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R"] = "右中指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R"] = "右中指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R"] = "右中指３",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R"] = "右親指１",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R"] = "右親指２",
            ["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R"] = "右親指３"
        };


    }
}
