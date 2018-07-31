#define TRANSFORM_60FPS_TO_30FPS
//#undef TRANSFORM_60FPS_TO_30FPS
#define SCALE_TO_VMD_SIZE
//#undef SCALE_TO_VMD_SIZE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Pmx;
using MillionDance.Entities.Vmd;
using MillionDance.Extensions;
using MillionDance.Utilities;
using OpenTK;
using UnityStudio.UnityEngine.Animation;

namespace MillionDance.Core {
    public static class VmdCreator {

        [NotNull]
        public static VmdMotion FromDanceData([NotNull] CharacterImasMotionAsset dance, [NotNull] Avatar avatar, [NotNull] PmxModel pmx) {
            var mltdHierarchy = BoneUtils.BuildBoneHierarchy(avatar);
            var pmxHierarchy = BoneUtils.BuildBoneHierarchy(pmx);

            Debug.Assert(mltdHierarchy.Count == pmxHierarchy.Count);

            foreach (var mltdBone in mltdHierarchy) {
                mltdBone.Initialize();
            }

            foreach (var pmxBone in pmxHierarchy) {
                pmxBone.Initialize();
            }

            var animation = BodyAnimation.CreateFrom(dance);
            var boneCount = mltdHierarchy.Count;
            var animatedBoneCount = animation.BoneCount;
            var keyFrameCount = animation.KeyFrames.Count;

            Debug.Assert(keyFrameCount % animatedBoneCount == 0, "keyFrameCount % animatedBoneCount == 0");

            var iterationTimes = keyFrameCount / animatedBoneCount;
            var boneFrameList = new List<VmdBoneFrame>();

            for (var i = 0; i < iterationTimes; ++i) {
#if TRANSFORM_60FPS_TO_30FPS
                if (i % 2 == 1) {
                    continue;
                }
#endif

                var keyFrameIndexStart = i * animatedBoneCount;

                for (var j = 0; j < animatedBoneCount; ++j) {
                    var keyFrame = animation.KeyFrames[keyFrameIndexStart + j];
                    var mltdBoneName = keyFrame.Path.Replace("BODY_SCALE/", string.Empty);
                    var targetBone = mltdHierarchy.SingleOrDefault(bone => bone.Name == mltdBoneName);

                    if (targetBone == null) {
                        throw new ArgumentException("Bone not found.");
                    }

                    if (keyFrame.HasPositions) {
                        var x = keyFrame.PositionX.Value;
                        var y = keyFrame.PositionY.Value;
                        var z = keyFrame.PositionZ.Value;

                        var t = new Vector3(x, y, z);

                        t = t.FixUnityToOpenTK();

#if SCALE_TO_VMD_SIZE
                        t = t * ConversionConfig.ScaleUnityToMmd;
#endif

                        targetBone.LocalPosition = t;
                    }

                    if (keyFrame.HasRotations) {
                        var x = keyFrame.AngleX.Value;
                        var y = keyFrame.AngleY.Value;
                        var z = keyFrame.AngleZ.Value;

                        var q = UnityRotation.EulerDeg(x, y, z);

                        q = q.FixUnityToOpenTK();

                        targetBone.LocalRotation = q;
                    }
                }

                foreach (var mltdBone in mltdHierarchy) {
                    mltdBone.UpdateTransform();
                }

                for (var j = 0; j < boneCount; ++j) {
                    var mltdBone = mltdHierarchy[j];
                    var pmxBone = pmxHierarchy[j];

                    var skinMatrix = mltdBone.SkinMatrix;
                    var mPmxBindingPose = pmxBone.BindingPose;
                    var mWorld = pmxBone.Parent?.WorldMatrix ?? Matrix4.Identity;

                    // skinMatrix == inv(mPmxBindingPose) x mLocal x mWorld
                    var mLocal = mPmxBindingPose * skinMatrix * mWorld.Inverted();

                    // Here, translation is in... world coords? WTF?
                    var t = mLocal.ExtractTranslation();
                    var q = mLocal.ExtractRotation();

                    if (pmxBone.Parent != null) {
                        t = t - (pmxBone.InitialPosition - pmxBone.Parent.InitialPosition);
                    }

#if TRANSFORM_60FPS_TO_30FPS
                    var frameIndex = i / 2;
#else
                    var frameIndex = i;
#endif

                    var vmdBoneName = GetVmdBoneNameFromBoneName(mltdBone.Path);
                    var boneFrame = new VmdBoneFrame(frameIndex, vmdBoneName);

                    boneFrame.Position = t;
                    boneFrame.Rotation = q;

                    boneFrameList.Add(boneFrame);

                    pmxBone.LocalPosition = t;
                    pmxBone.LocalRotation = q;
                    pmxBone.UpdateTransform();
                }
            }

            const string modelName = "MODEL_00";

            var vmd = new VmdMotion(modelName, boneFrameList, ArrayCache.Empty<VmdFacialFrame>(), ArrayCache.Empty<VmdCameraFrame>(), ArrayCache.Empty<VmdLightFrame>(), null);

            return vmd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetVmdBoneNameFromBonePath([NotNull] string mltdBonePath) {
            return GetBoneName(BoneUtils.BonePathMap, mltdBonePath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetVmdBoneNameFromBoneName([NotNull] string mltdBoneName) {
            return GetBoneName(BoneUtils.BoneNameMap, mltdBoneName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetBoneName([NotNull] IReadOnlyDictionary<string, string> nameDict, [NotNull] string key) {
            if (nameDict.ContainsKey(key)) {
                return nameDict[key];
            } else {
                return $"Bone #{key.GetHashCode():x8}";
            }
        }

    }
}
