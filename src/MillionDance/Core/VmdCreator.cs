#define TRANSFORM_60FPS_TO_30FPS
//#undef TRANSFORM_60FPS_TO_30FPS
#define SCALE_TO_VMD_SIZE
//#undef SCALE_TO_VMD_SIZE
#define APPEND_IK_BONES
//#undef APPEND_IK_BONES

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
        public static VmdMotion CreateFrom([CanBeNull] CharacterImasMotionAsset bodyMotion, [CanBeNull] CharacterImasMotionAsset cameraMotion, [NotNull] Avatar avatar, [NotNull] PmxModel mltdPmxModel) {
            IReadOnlyList<VmdBoneFrame> boneFrames;
            IReadOnlyList<VmdCameraFrame> cameraFrames;

            if (bodyMotion != null) {
                boneFrames = CreateBodyFrames(bodyMotion, avatar, mltdPmxModel);
            } else {
                boneFrames = ArrayCache.Empty<VmdBoneFrame>();
            }

            if (cameraMotion != null) {
                cameraFrames = CreateCameraFrames(cameraMotion);
            } else {
                cameraFrames = ArrayCache.Empty<VmdCameraFrame>();
            }

            const string modelName = "MODEL_00";

            var vmd = new VmdMotion(modelName, boneFrames, ArrayCache.Empty<VmdFacialFrame>(), cameraFrames, ArrayCache.Empty<VmdLightFrame>(), null);

            return vmd;
        }

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdBoneFrame> CreateBodyFrames([NotNull] CharacterImasMotionAsset bodyMotion, [NotNull] Avatar avatar, [NotNull] PmxModel pmx) {
            var mltdHierarchy = BoneUtils.BuildBoneHierarchy(avatar);
            var pmxHierarchy = BoneUtils.BuildBoneHierarchy(pmx);

#if APPEND_IK_BONES
            throw new NotSupportedException("Not supported when appending IK is enabled.");
#else
            Debug.Assert(mltdHierarchy.Count == pmxHierarchy.Count);
#endif

            foreach (var mltdBone in mltdHierarchy) {
                mltdBone.Initialize();
            }

            foreach (var pmxBone in pmxHierarchy) {
                pmxBone.Initialize();
            }

            var animation = BodyAnimation.CreateFrom(bodyMotion);
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
                        //throw new ArgumentException("Bone not found.");
                        continue; // Shika doesn't have the "POSITION" bone.
                    }

                    BoneNode transferredBone = null;

                    foreach (var kv in BoneAttachmentMap) {
                        if (kv.Key == mltdBoneName) {
                            transferredBone = mltdHierarchy.SingleOrDefault(bone => bone.Name == kv.Value);

                            if (transferredBone == null) {
                                throw new ArgumentException();
                            }

                            break;
                        }
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

                        //if (transferredBone != null) {
                        //    transferredBone.LocalPosition = t;
                        //}
                    }

                    if (keyFrame.HasRotations) {
                        var x = keyFrame.AngleX.Value;
                        var y = keyFrame.AngleY.Value;
                        var z = keyFrame.AngleZ.Value;

                        var q = UnityRotation.EulerDeg(x, y, z);

                        q = q.FixUnityToOpenTK();

                        targetBone.LocalRotation = q;

                        if (transferredBone != null) {
                            transferredBone.LocalRotation = q;
                        }
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

            return boneFrameList;
        }

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdCameraFrame> CreateCameraFrames([NotNull] CharacterImasMotionAsset cameraMotion) {
            var animation = CameraAnimation.CreateFrom(cameraMotion);
            var animationFrameCount = animation.CameraFrames.Count;

            var cameraFrameList = new List<VmdCameraFrame>();

            for (var i = 0; i < animationFrameCount; ++i) {
#if TRANSFORM_60FPS_TO_30FPS
                if (i % 2 == 1) {
                    continue;
                }
#endif

#if TRANSFORM_60FPS_TO_30FPS
                var frameIndex = i / 2;
#else
                var frameIndex = i;
#endif

                var frame = animation.CameraFrames[i];
                var vmdFrame = new VmdCameraFrame(frameIndex);

                var pos = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                pos = pos.FixUnityToOpenTK();

#if SCALE_TO_VMD_SIZE
                pos = pos * ConversionConfig.ScaleUnityToMmd;
#endif

                vmdFrame.Position = pos;

                var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                target = target.FixUnityToOpenTK();

#if SCALE_TO_VMD_SIZE
                target = target * ConversionConfig.ScaleUnityToMmd;
#endif

                var delta = target - pos;

                vmdFrame.Length = delta.Length;

                var qy = -(float)Math.Atan2(delta.X, delta.Z);
                var qx = (float)Math.Atan2(delta.Y, delta.Z);
                var qz = 0f;

                qx += MathHelper.DegreesToRadians(frame.AngleX);
                qy += MathHelper.DegreesToRadians(frame.AngleY);
                qz += MathHelper.DegreesToRadians(frame.AngleZ);

                if (qx < -MathHelper.PiOver2) {
                    qx = qx + MathHelper.Pi;
                } else if (qx > MathHelper.PiOver2) {
                    qx = MathHelper.Pi - qx;
                }

                if (qz < -MathHelper.PiOver2) {
                    qz = qz + MathHelper.Pi;
                } else if (qz > MathHelper.PiOver2) {
                    qz = MathHelper.Pi - qz;
                }

                vmdFrame.Orientation = new Vector3(qx, qy, qz);

                // VMD does not have good support for animated FOV. So here just use a constant to avoid "jittering".
                // The drawback is, some effects (like the first zooming cut in Shooting Stars) will not be able to achieve.
                //var fov = FocalLengthToFov(frame.FocalLength);
                //vmdFrame.FieldOfView = (uint)fov;

                const uint constFov = 20;
                vmdFrame.FieldOfView = constFov;

                cameraFrameList.Add(vmdFrame);
            }

            return cameraFrameList;
        }

        // https://photo.stackexchange.com/questions/41273/how-to-calculate-the-fov-in-degrees-from-focal-length-or-distance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float FocalLengthToFov(float focalLength) {
            // By experiments, MLTD seems to use a 15mm or 16mm camera.
            const float sensorSize = 15; // unit: mm, as the unit of MLTD camera frame is also mm
            var fovRad = 2 * (float)Math.Atan((sensorSize / 2) / focalLength);
            var fovDeg = MathHelper.RadiansToDegrees(fovRad);

            return fovDeg;
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

        private static readonly IReadOnlyDictionary<string, string> BoneAttachmentMap = new Dictionary<string, string> {
            //["MODEL_00/BASE/MUNE1/MUNE2/KUBI"] = "KUBI",
            ["MODEL_00/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "KUBI/ATAMA"
        };

    }
}
