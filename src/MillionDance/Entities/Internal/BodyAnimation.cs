using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AssetStudio;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Extensions;
using OpenMLTD.MillionDance.Extensions;

namespace OpenMLTD.MillionDance.Entities.Internal {
    public sealed class BodyAnimation {

        private BodyAnimation([NotNull, ItemNotNull] KeyFrame[] keyFrames, float duration, int frameCount, int boneCount) {
            KeyFrames = keyFrames;
            Duration = duration;
            FrameCount = frameCount;
            BoneCount = boneCount;
        }

        /// <summary>
        /// All keyframes, stored in "batches". Each "batch" contains frames whose number equals <see cref="BoneCount"/>.
        /// </summary>
        [NotNull, ItemNotNull]
        public KeyFrame[] KeyFrames { get; }

        public float Duration { get; }

        public int BoneCount { get; }

        /// <summary>
        /// The number of animation frames. Each frame represents a complete snapshot of the model state.
        /// </summary>
        public int FrameCount { get; }

        [NotNull]
        internal static BodyAnimation CreateFrom([NotNull] CharacterImasMotionAsset danceMotion) {
            var (frames, frameCount, boneCount) = ComputeKeyFrames(danceMotion.Curves);

            return new BodyAnimation(frames, danceMotion.Duration, frameCount, boneCount);
        }

        [NotNull]
        internal static BodyAnimation CreateFrom([NotNull] AnimationClip danceAnimationClip) {
            // MLTD uses a type-0 animation clip (seems to be full-frame generic)
            Debug.Assert(danceAnimationClip.m_AnimationType == 0);

            var baseClip = danceAnimationClip.m_MuscleClip.m_Clip;

            Debug.Assert(baseClip != null, nameof(baseClip) + " != null");

            var streamedClip = baseClip.m_StreamedClip;

            Debug.Assert(streamedClip != null, nameof(streamedClip) + " != null");
            Debug.Assert(streamedClip.curveCount == CurvesInAnimationClip, "streamedClip.curveCount == CurvesInAnimationClip");

            var streamedFrames = streamedClip.ReadData();

            var denseClip = baseClip.m_DenseClip;

            Debug.Assert(denseClip != null, nameof(denseClip) + " != null");

            var frameCount = denseClip.m_FrameCount;

            Debug.Assert(frameCount > 0, nameof(frameCount) + " > 0");

            // TODO: This method seems extremely slow (we can directly convert from AnimationClip's frames) but it reuses code. Maybe do some cleanup in the future.

            var curves = new Curve[CurvesInAnimationClip];

            for (var i = 0; i < CurvesInAnimationClip; i += 1) {
                var curve = new Curve();
                var (path, propType) = CurveTable[i];

                curve.Path = path;
                curve.Attributes = new[] {
                    propType.ToAttributeTextFast(),
                    KeyType.FullFrame.ToAttributeTextFast(),
                };

                var values = new float[frameCount];

                for (var j = 0; j < frameCount; j += 1) {
                    var curveKey = streamedFrames[j].keyList[i];
                    Debug.Assert(curveKey.index == i);
                    values[j] = curveKey.value;
                }

                curve.Values = values;

                curves[i] = curve;
            }

            var (frames, _, boneCount) = ComputeKeyFrames(curves);

            var frameRate = denseClip.m_SampleRate;
            var duration = frameCount / frameRate;

            return new BodyAnimation(frames, duration, frameCount, boneCount);
        }

        private static (KeyFrame[] Frames, int FrameCount, int BoneCount) ComputeKeyFrames([NotNull, ItemNotNull] Curve[] curves) {
            var frameCount = curves.Max(curve => curve.Values.Length);
            var frameDict = new Dictionary<string, List<KeyFrame>>();

            foreach (var curve in curves) {
                List<KeyFrame> frameList;

                if (frameDict.ContainsKey(curve.Path)) {
                    frameList = frameDict[curve.Path];
                } else {
                    frameList = new List<KeyFrame>();
                    frameDict.Add(curve.Path, frameList);
                }

                var path = curve.Path;
                var keyType = curve.GetKeyType();
                var propertyType = curve.GetPropertyType();

                switch (keyType) {
                    case KeyType.Const: {
                        switch (propertyType) {
                            case PropertyType.AngleX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).AngleX = curve.Values[0];
                                }

                                break;
                            }
                            case PropertyType.AngleY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).AngleY = curve.Values[0];
                                }

                                break;
                            }
                            case PropertyType.AngleZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).AngleZ = curve.Values[0];
                                }

                                break;
                            }
                            case PropertyType.PositionX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).PositionX = curve.Values[0];
                                }

                                break;
                            }
                            case PropertyType.PositionY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).PositionY = curve.Values[0];
                                }

                                break;
                            }
                            case PropertyType.PositionZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    GetOrAddFrame(frameList, path, frameIndex).PositionZ = curve.Values[0];
                                }

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                        }
                    }
                        break;
                    case KeyType.FullFrame: {
                        var valueCount = curve.Values.Length;

                        switch (propertyType) {
                            case PropertyType.AngleX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).AngleX = curve.Values[index];
                                }

                                break;
                            }
                            case PropertyType.AngleY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).AngleY = curve.Values[index];
                                }

                                break;
                            }
                            case PropertyType.AngleZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).AngleZ = curve.Values[index];
                                }

                                break;
                            }
                            case PropertyType.PositionX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).PositionX = curve.Values[index];
                                }

                                break;
                            }
                            case PropertyType.PositionY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).PositionY = curve.Values[index];
                                }

                                break;
                            }
                            case PropertyType.PositionZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                    GetOrAddFrame(frameList, path, frameIndex).PositionZ = curve.Values[index];
                                }

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                        }
                    }

                        break;
                    case KeyType.Discrete: {
                        if ((curve.Values.Length % 2) != 0) {
                            throw new ApplicationException($"Length of curve values {curve.Values.Length} is not a multiple of 2.");
                        }

                        var curveValueCount = curve.Values.Length / 2;
                        var curTime = curve.Values[0];
                        var curValue = curve.Values[1];
                        var nextTime = curve.Values[2];
                        var nextValue = curve.Values[3];
                        var curIndex = 0;

                        var curve1 = curve;

                        float InterpolateValue(KeyFrame frame) {
                            if (curIndex >= curveValueCount - 1) {
                                return curValue;
                            }

                            var frameTime = frame.Time;

                            if (frameTime >= nextTime) {
                                curTime = nextTime;
                                curValue = nextValue;
                                ++curIndex;

                                if (curIndex < curveValueCount - 1) {
                                    nextTime = curve1.Values[(curIndex + 1) * 2];
                                    nextValue = curve1.Values[(curIndex + 1) * 2 + 1];
                                }
                            }

                            if (curIndex >= curveValueCount - 1) {
                                return curValue;
                            }

                            var duration = nextTime - curTime;
                            var delta = frameTime - curTime;
                            var p = delta / duration;

                            return curValue * (1 - p) + nextValue * p;
                        }

                        switch (propertyType) {
                            case PropertyType.AngleX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.AngleX = value;
                                }

                                break;
                            }
                            case PropertyType.AngleY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.AngleY = value;
                                }

                                break;
                            }
                            case PropertyType.AngleZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.AngleZ = value;
                                }

                                break;
                            }
                            case PropertyType.PositionX: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.PositionX = value;
                                }

                                break;
                            }
                            case PropertyType.PositionY: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.PositionY = value;
                                }

                                break;
                            }
                            case PropertyType.PositionZ: {
                                for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                    var frame = GetOrAddFrame(frameList, path, frameIndex);
                                    var value = InterpolateValue(frame);
                                    frame.PositionZ = value;
                                }

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                        }
                    }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(keyType), keyType, $"Unknown key type: \"{keyType}\".");
                }
            }

            var totalList = frameDict.SelectManyToList(kv => kv.Value);

            totalList.Sort((f1, f2) => {
                var s1 = f1.FrameIndex.CompareTo(f2.FrameIndex);

                if (s1 != 0) {
                    return s1;
                }

                return string.Compare(f1.Path, f2.Path, StringComparison.Ordinal);
            });

            return (totalList.ToArray(), frameCount, frameDict.Count);
        }

        [NotNull]
        private static KeyFrame GetOrAddFrame([NotNull, ItemNotNull] List<KeyFrame> frameList, [NotNull] string path, int index) {
            KeyFrame frame;

            if (frameList.Count > index) {
                frame = frameList[index];
            } else {
                frame = new KeyFrame(index, path);
                frameList.Add(frame);
            }

            return frame;
        }

        private const int CurvesInAnimationClip = 180;

        [NotNull]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        [SuppressMessage("ReSharper", "CommentTypo")]
        private static readonly (string Path, PropertyType PropertyType)[] CurveTable = {
            // Bone curve order in legacy motion files (dan_*.imo.unity3d)
            // When converting the legacy files, we read the order stored in the files directly so we don't use this table.

            // ("POSITION", PropertyType.AngleX),
            // ("POSITION", PropertyType.AngleY),
            // ("POSITION", PropertyType.AngleZ),
            // ("MODEL_00", PropertyType.AngleX),
            // ("MODEL_00", PropertyType.AngleY),
            // ("MODEL_00", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleZ),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleX),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleY),
            // ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleZ),
            // ("POSITION", PropertyType.PositionX),
            // ("POSITION", PropertyType.PositionY),
            // ("POSITION", PropertyType.PositionZ),
            // ("POSITION/SCALE_POINT", PropertyType.PositionX),
            // ("POSITION/SCALE_POINT", PropertyType.PositionY),
            // ("POSITION/SCALE_POINT", PropertyType.PositionZ),
            // ("MODEL_00", PropertyType.PositionX),
            // ("MODEL_00", PropertyType.PositionY),
            // ("MODEL_00", PropertyType.PositionZ),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionX),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionY),
            // ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionZ),

            // Bone curve in newer animations

            ("POSITION", PropertyType.PositionX),
            ("POSITION", PropertyType.PositionY),
            ("POSITION", PropertyType.PositionZ),
            ("POSITION/SCALE_POINT", PropertyType.PositionX),
            ("POSITION/SCALE_POINT", PropertyType.PositionY),
            ("POSITION/SCALE_POINT", PropertyType.PositionZ),
            ("MODEL_00", PropertyType.PositionX),
            ("MODEL_00", PropertyType.PositionY),
            ("MODEL_00", PropertyType.PositionZ),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionX),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionY),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.PositionZ),
            ("POSITION", PropertyType.AngleX),
            ("POSITION", PropertyType.AngleY),
            ("POSITION", PropertyType.AngleZ),
            ("MODEL_00", PropertyType.AngleX),
            ("MODEL_00", PropertyType.AngleY),
            ("MODEL_00", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/OYA3_L/OYA2_L/OYA1_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/HITO3_L/HITO2_L/HITO1_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/NAKA3_L/NAKA2_L/NAKA1_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KUSU3_L/KUSU2_L/KUSU1_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_L/KATA_L/UDE_L/TE_L/KUKO_L/KO3_L/KO2_L/KO1_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/OYA3_R/OYA2_R/OYA1_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/HITO3_R/HITO2_R/HITO1_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/NAKA3_R/NAKA2_R/NAKA1_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KUSU3_R/KUSU2_R/KUSU1_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/SAKOTSU_R/KATA_R/UDE_R/TE_R/KUKO_R/KO3_R/KO2_R/KO1_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/MUNE1/MUNE2/KUBI/ATAMA", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L/HIZA_L/ASHI_L/TSUMASAKI_L", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R", PropertyType.AngleZ),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleX),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleY),
            ("MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_R/HIZA_R/ASHI_R/TSUMASAKI_R", PropertyType.AngleZ),
        };

    }
}
