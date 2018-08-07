using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class VmdCreator {

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        [NotNull]
        public VmdMotion CreateFrom([CanBeNull] CharacterImasMotionAsset bodyMotion, [NotNull] Avatar avatar, [NotNull] PmxModel mltdPmxModel,
            [CanBeNull] CharacterImasMotionAsset cameraMotion,
            [CanBeNull] ScenarioObject scenarioObject, int songPosition) {
            IReadOnlyList<VmdBoneFrame> boneFrames;
            IReadOnlyList<VmdCameraFrame> cameraFrames;
            IReadOnlyList<VmdFacialFrame> facialFrames;
            IReadOnlyList<VmdLightFrame> lightFrames;

            if (ProcessBoneFrames && bodyMotion != null) {
                boneFrames = CreateBoneFrames(bodyMotion, avatar, mltdPmxModel);
            } else {
                boneFrames = EmptyArray.Of<VmdBoneFrame>();
            }

            if (ProcessCameraFrames && cameraMotion != null) {
                cameraFrames = CreateCameraFrames(cameraMotion);
            } else {
                cameraFrames = EmptyArray.Of<VmdCameraFrame>();
            }

            if (ProcessFacialFrames && scenarioObject != null) {
                facialFrames = CreateFacialFrames(scenarioObject, songPosition);
            } else {
                facialFrames = EmptyArray.Of<VmdFacialFrame>();
            }

            if (ProcessLightFrames && scenarioObject != null) {
                lightFrames = CreateLightFrames(scenarioObject);
            } else {
                lightFrames = EmptyArray.Of<VmdLightFrame>();
            }

            const string modelName = "MODEL_00";

            var vmd = new VmdMotion(modelName, boneFrames, facialFrames, cameraFrames, lightFrames, null);

            return vmd;
        }

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdBoneFrame> CreateBoneFrames([NotNull] CharacterImasMotionAsset bodyMotion, [NotNull] Avatar avatar, [NotNull] PmxModel pmx) {
            var mltdHierarchy = BoneUtils.BuildBoneHierarchy(avatar);
            var pmxHierarchy = BoneUtils.BuildBoneHierarchy(pmx);

            if (ConversionConfig.Current.AppendIKBones || ConversionConfig.Current.AppendEyeBones) {
                throw new NotSupportedException("Not supported when appending bones is enabled.");
            } else {
                Debug.Assert(mltdHierarchy.Count == pmxHierarchy.Count);
            }

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
                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    if (i % 2 == 1) {
                        continue;
                    }
                }

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

                        if (ConversionConfig.Current.ScaleToVmdSize) {
                            t = t * ScalingConfig.ScaleUnityToMmd;
                        }

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

                    int frameIndex;

                    if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                        frameIndex = i / 2;
                    } else {
                        frameIndex = i;
                    }

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
                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    if (i % 2 == 1) {
                        continue;
                    }
                }

                int frameIndex;

                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    frameIndex = i / 2;
                } else {
                    frameIndex = i;
                }

                var frame = animation.CameraFrames[i];
                var vmdFrame = new VmdCameraFrame(frameIndex);

                var pos = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                pos = pos.FixUnityToOpenTK();

                if (ConversionConfig.Current.ScaleToVmdSize) {
                    pos = pos * ScalingConfig.ScaleUnityToMmd;
                }

                vmdFrame.Position = pos;

                var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                target = target.FixUnityToOpenTK();

                if (ConversionConfig.Current.ScaleToVmdSize) {
                    target = target * ScalingConfig.ScaleUnityToMmd;
                }

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

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdFacialFrame> CreateFacialFrames([NotNull] ScenarioObject scenarioObject, int songPosition) {
            VmdFacialFrame CreateFacialFrame(float time, string mltdTruncMorphName, float value) {
                var n = (int)(time * 60.0f);
                int frameIndex;

                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    frameIndex = n / 2;
                } else {
                    frameIndex = n;
                }

                string expressionName;

                if (ConversionConfig.Current.TranslateFacialExpressionNamesToMmd) {
                    expressionName = MorphUtils.LookupMorphName(mltdTruncMorphName);
                } else {
                    expressionName = mltdTruncMorphName;
                }

                var frame = new VmdFacialFrame(frameIndex, expressionName);
                frame.Weight = value;

                return frame;
            }

            var facialFrameList = new List<VmdFacialFrame>();

            // Lip motion
            {
                var lipSyncControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.LipSync).ToArray();

                Debug.Assert(lipSyncControls.Length > 0);
                Debug.Assert(lipSyncControls[0].Param == 54);
                Debug.Assert(lipSyncControls[lipSyncControls.Length - 1].Param == 54);

                const float lipTransitionTime = 0.2f;
                float lastLipSyncTime = 0;

                for (var i = 0; i < lipSyncControls.Length; i++) {
                    var sync = lipSyncControls[i];
                    var currentTime = (float)sync.AbsoluteTime;
                    var hasNext = i < lipSyncControls.Length - 1;

                    var nextTime = hasNext ? (float)lipSyncControls[i + 1].AbsoluteTime : 0.0f;

                    switch (sync.Param) {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 50:
                            Debug.Assert(hasNext);

                            string morphName;

                            switch (sync.Param) {
                                case 0:
                                    morphName = "M_a";
                                    break;
                                case 1:
                                    morphName = "M_i";
                                    break;
                                case 2:
                                    morphName = "M_u";
                                    break;
                                case 3:
                                    morphName = "M_e";
                                    break;
                                case 4:
                                    morphName = "M_o";
                                    break;
                                case 50:
                                    morphName = "M_n";
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("Not possible.");
                            }

                            facialFrameList.Add(CreateFacialFrame(currentTime - lipTransitionTime, morphName, 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, morphName, 1));
                            facialFrameList.Add(CreateFacialFrame(nextTime - lipTransitionTime, morphName, 1));
                            facialFrameList.Add(CreateFacialFrame(nextTime, morphName, 0));
                            break;
                        case 54:
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_a", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_i", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_u", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_e", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_o", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_n", 0));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(sync.Param), sync.Param, null);
                    }
                }
            }

            // Facial expression
            {
                var expControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.FacialExpression).ToArray();
                var eyeClosed = false;

                // Note that here we don't process blinkings (which happens in MLTD)
                for (var i = 0; i < expControls.Length; i++) {
                    var exp = expControls[i];
                    var currentTime = (float)exp.AbsoluteTime;

                    const float eyeBlinkTime = 0.1f;
                    const float faceTransitionTime = 0.1333333f;

                    eyeClosed = exp.EyeClosed;

                    var eyesClosedRatio = exp.EyeClosed ? 1.0f : 0.0f;

                    facialFrameList.Add(CreateFacialFrame(currentTime, "E_metoji_r", eyesClosedRatio));
                    facialFrameList.Add(CreateFacialFrame(currentTime, "E_metoji_l", eyesClosedRatio));

                    if (i > 0) {
                        if (expControls[i - 1].EyeClosed != exp.EyeClosed) {
                            facialFrameList.Add(CreateFacialFrame(currentTime - eyeBlinkTime, "E_metoji_r", 1 - eyesClosedRatio));
                            facialFrameList.Add(CreateFacialFrame(currentTime - eyeBlinkTime, "E_metoji_l", 1 - eyesClosedRatio));
                        }
                    }

                    var expressionKey = (FacialExpression)exp.Param;

                    if (FacialExpressionTable.ContainsKey(expressionKey)) {
                        foreach (var kv in FacialExpressionTable[expressionKey]) {
                            if ((kv.Key != "E_metoji_r" && kv.Key != "E_metoji_l") || !eyeClosed) {
                                facialFrameList.Add(CreateFacialFrame(currentTime, kv.Key, kv.Value));
                            }
                        }
                    } else {
                        Debug.Print("Warning: facial expression key not found: {0}", exp.Param);
                    }

                    if (i > 0) {
                        if (expControls[i - 1].Param != exp.Param) {
                            var lastExpressionKey = (FacialExpression)expControls[i - 1].Param;

                            if (FacialExpressionTable.ContainsKey(lastExpressionKey)) {
                                foreach (var kv in FacialExpressionTable[lastExpressionKey]) {
                                    // TODO: Actually we should do a more thorough analysis, because in this time window the eye CAN be opened again so we actually need these values.
                                    // But whatever. This case is rare. Fix it in the future.
                                    if ((kv.Key != "E_metoji_r" && kv.Key != "E_metoji_l") || !eyeClosed) {
                                        facialFrameList.Add(CreateFacialFrame(currentTime - faceTransitionTime, kv.Key, kv.Value));
                                    }
                                }
                            } else {
                                Debug.Print("Warning: facial expression key not found: {0}", expControls[i - 1].Param);
                            }
                        }
                    }
                }
            }

            return facialFrameList;
        }

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdLightFrame> CreateLightFrames([NotNull] ScenarioObject scenarioObject) {
            var lightFrameList = new List<VmdLightFrame>();
            var lightControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.SetLightColor).ToArray();

            foreach (var lightControl in lightControls) {
                var n = (int)((float)lightControl.AbsoluteTime * 60.0f);
                int frameIndex;

                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    frameIndex = n / 2;
                } else {
                    frameIndex = n;
                }

                var frame = new VmdLightFrame(frameIndex);

                frame.Position = new Vector3(0.5f, -1f, -0.5f);

                var c = lightControl.Color;
                frame.Color = new Vector3(c.R, c.G, c.B);

                lightFrameList.Add(frame);
            }

            return lightFrameList;
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
