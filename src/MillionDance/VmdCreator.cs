#define TRANSFORM_60_TO_30
#define FIX_FRAMES

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Pmx;
using MillionDance.Entities.Vmd;
using MillionDance.Extensions;
using OpenTK;
using UnityStudio.Unity.Animation;

namespace MillionDance {
    public static class VmdCreator {

        [NotNull]
        public static VmdMotion FromDanceData([NotNull] CharacterImasMotionAsset dance, [NotNull] Avatar avatar, [NotNull] PmxModel pmx) {
            const string modelName = "MODEL_00";

            var boneFrames = new List<VmdBoneFrame>();

            var anim = Animation.CreateFrom(dance);

            //var maxx = anim.KeyFrames.Max(f => f.AngleX ?? 0);
            //var maxy = anim.KeyFrames.Max(f => f.AngleY ?? 0);
            //var maxz = anim.KeyFrames.Max(f => f.AngleZ ?? 0);
            //var minx = anim.KeyFrames.Min(f => f.AngleX ?? 0);
            //var miny = anim.KeyFrames.Min(f => f.AngleY ?? 0);
            //var minz = anim.KeyFrames.Min(f => f.AngleZ ?? 0);

#if TRANSFORM_60_TO_30
            var counter = 0;
#endif

#if FIX_FRAMES
            var invMltdQs = new Dictionary<string, Quaternion>();
            var pmxQs = new Dictionary<string, Quaternion>();

            foreach (var kv in BoneNameMap) {
                var mltdPathName = kv.Key;
                var vmdName = kv.Value;

                var mltdBoneName = mltdPathName.Replace("BODY_SCALE/", string.Empty);

                if (pmx.BonesDictionary.ContainsKey(vmdName)) {
                    var mltdBoneId = avatar.BoneNamesMap.Single(nn => nn.Value == mltdBoneName).Key;
                    var mltdBoneIndex = avatar.AvatarSkeleton.NodeIDs.FindIndex(mltdBoneId);

                    if (mltdBoneIndex >= 0) {
                        var mltdBone = avatar.AvatarSkeletonPose.Transforms[mltdBoneIndex];

                        var mltdQ = mltdBone.Rotation.ToOpenTK();

                        invMltdQs[mltdPathName] = Quaternion.Invert(mltdQ);
                    } else {
                        invMltdQs[mltdPathName] = Quaternion.Identity;
                    }

                    var pmxBone = pmx.BonesDictionary[vmdName];

                    pmxQs[mltdPathName] = pmxBone.InitialRotation;
                } else {
                    // Assumes no difference
                    invMltdQs[mltdPathName] = Quaternion.Identity;
                    pmxQs[mltdPathName] = Quaternion.Identity;
                }
            }
#endif

            foreach (var keyFrame in anim.KeyFrames) {
#if TRANSFORM_60_TO_30
                // MLTD 60 fps -> VMD 30 fps
                ++counter;

                if (counter % 2 == 0) {
                    continue;
                }
#endif

                var boneName = BoneNameMap.ContainsKey(keyFrame.Path) ? BoneNameMap[keyFrame.Path] : "UNDEFINED";

                var boneFrame = new VmdBoneFrame(keyFrame.FrameIndex / 2, boneName);

                if (keyFrame.HasPositions) {
                    const float vmdScale = 0.08f;

                    Debug.Assert(keyFrame.PositionX != null, "keyFrame.PositionX != null");
                    var x = keyFrame.PositionX.Value;
                    Debug.Assert(keyFrame.PositionY != null, "keyFrame.PositionY != null");
                    var y = keyFrame.PositionY.Value;
                    Debug.Assert(keyFrame.PositionZ != null, "keyFrame.PositionZ != null");
                    var z = keyFrame.PositionZ.Value;
                    boneFrame.Position = new Vector3(-x, y, z) / vmdScale;
                }

                if (keyFrame.HasRotations) {
                    Debug.Assert(keyFrame.AngleX != null, "keyFrame.AngleX != null");
                    var ax = MathHelper.DegreesToRadians(keyFrame.AngleX.Value);
                    Debug.Assert(keyFrame.AngleY != null, "keyFrame.AngleY != null");
                    var ay = MathHelper.DegreesToRadians(keyFrame.AngleY.Value);
                    Debug.Assert(keyFrame.AngleZ != null, "keyFrame.AngleZ != null");
                    var az = MathHelper.DegreesToRadians(keyFrame.AngleZ.Value);

                    var q = Quaternion.FromEulerAngles(ax, ay, az);

#if FIX_FRAMES
                    var invMltdQ = invMltdQs[keyFrame.Path];

                    q = invMltdQ * q;

                    q = MltdQSpaceToVmdQSpace(q);

                    var pmxQ = pmxQs[keyFrame.Path];

                    q = pmxQ * q;
#endif

                    //q.X = -q.X;
                    //q.Y = -q.Y;
                    //q.Z = -q.Z;

                    boneFrame.Rotation = q;
                }

                boneFrames.Add(boneFrame);
            }

            var facialFrames = new VmdFacialFrame[0];
            var cameraFrames = new VmdCameraFrame[0];
            var lightFrames = new VmdLightFrame[0];

            return new VmdMotion(modelName, boneFrames.ToArray(), facialFrames, cameraFrames, lightFrames, null);
        }

        // https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles#Quaternion_to_Euler_Angles_Conversion
        // https://forum.unity.com/threads/right-hand-to-left-handed-conversions.80679/
        // https://www.gamedev.net/forums/topic/349800-convert-rotations-from-right-hand-to-left-hand/
        // https://www.codeproject.com/Tips/1240454/How-to-Convert-Right-Handed-to-Left-Handed-Coordin
        // https://stackoverflow.com/questions/28673777/convert-quaternion-from-right-handed-to-left-handed-coordinate-system
        // https://www.gamedev.net/forums/topic/654682-quaternions-convert-between-left-right-handed-without-using-euler/
        private static Quaternion MltdQSpaceToVmdQSpace(Quaternion q) {
            // TODO: Something here is wrong.
            var rotVec = QtoV(q);

            rotVec.X = -rotVec.X;
            //rotVec.Y = -rotVec.Y;
            rotVec.Z = -rotVec.Z;

            return Quaternion.FromEulerAngles(rotVec);
        }

        private static Vector3 QtoV(Quaternion q) {
            var sinR = 2 * (q.W * q.X + q.Y * q.Z);
            var cosR = 1 - 2 * (q.X * q.X + q.Y * q.Y);

            var x = (float)Math.Atan2(sinR, cosR);

            var sinP = 2 * (q.W * q.Y - q.Z * q.X);

            float y;

            if (Math.Abs(sinP) >= 1) {
                y = Math.Sign(sinP) * MathHelper.PiOver2;
            } else {
                y = (float)Math.Asin(sinP);
            }

            var sinY = 2 * (q.W * q.Z + q.X * q.Y);
            var cosY = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);

            var z = (float)Math.Atan2(sinY, cosY);

            return new Vector3(x, y, z);
        }

        private static readonly IReadOnlyDictionary<string, string> BoneNameMap = new Dictionary<string, string> {
            //["POSITION"] = "操作中心",
            //["POSITION/SCALE_POINT"] = "全ての親",
            //["MODEL_00"] = "センター",
            //["MODEL_00/BODY_SCALE/BASE"] = "グルーブ",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI"] = "腰",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L"] = "左足",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L"] = "左ひざ",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L"] = "左足首",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L"] = "左つま先",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R"] = "右足",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R"] = "右ひざ",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R"] = "右足首",
            //["MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R"] = "右つま先",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1"] = "上半身",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2"] = "上半身2",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI"] = "首",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "頭",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L"] = "左肩",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L"] = "左腕",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L"] = "左ひじ",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L"] = "左手首",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L"] = "左人指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L"] = "左人指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L"] = "左人指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L"] = "左ダミー",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L"] = "左小指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L"] = "左小指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L"] = "左小指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L"] = "左薬指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L"] = "左薬指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L"] = "左薬指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L"] = "左中指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L"] = "左中指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L"] = "左中指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L"] = "左親指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L"] = "左親指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L"] = "左親指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R"] = "右肩",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R"] = "右腕",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R"] = "右ひじ",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R"] = "右手首",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R"] = "右人指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R"] = "右人指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R"] = "右人指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R"] = "右ダミー",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R"] = "右小指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R"] = "右小指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R"] = "右小指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R"] = "右薬指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R"] = "右薬指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R"] = "右薬指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R"] = "右中指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R"] = "右中指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R"] = "右中指３",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R"] = "右親指１",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R"] = "右親指２",
            //["MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R"] = "右親指３"
            ["POSITION"] = "操作中心",
            ["POSITION/SCALE_POINT"] = "全ての親",
            ["MODEL_00"] = "グルーブ",
            ["MODEL_00/BODY_SCALE/BASE"] = "腰",
            ["MODEL_00/BODY_SCALE/BASE/KOSHI"] = "UNDEF:腰",
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
