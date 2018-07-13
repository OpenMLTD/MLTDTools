using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MillionDance.Entities.Extensions;
using MillionDance.Entities.Unity;

namespace MillionDance.Entities.Internal {
    public sealed class Animation {

        private Animation([NotNull, ItemNotNull] KeyFrame[] keyFrames, TimeSpan duration) {
            KeyFrames = keyFrames;
            Duration = duration;
        }

        public IReadOnlyList<KeyFrame> KeyFrames { get; }

        public TimeSpan Duration { get; }

        public static Animation CreateFrom([NotNull] CharacterImasMotionAsset dance) {
            var curves = dance.Curves;

            var frameCount = curves.Max(curve => curve.Values.Length);
            var frameDict = new Dictionary<string, List<KeyFrame>>();

            for (var kk = 0; kk < curves.Length; ++kk) {
                var curve = curves[kk];

                List<KeyFrame> frameList;

                if (frameDict.ContainsKey(curve.Path)) {
                    frameList = frameDict[curve.Path];
                } else {
                    frameList = new List<KeyFrame>();
                    frameDict.Add(curve.Path, frameList);
                }

                var keyType = curve.GetKeyType();
                var propertyType = curve.GetPropertyType();

                KeyFrame GetOrAddFrame(int index, string path) {
                    KeyFrame frame;

                    if (frameList.Count > index) {
                        frame = frameList[index];
                    } else {
                        frame = new KeyFrame(index, path);
                        frameList.Add(frame);
                    }

                    return frame;
                }

                switch (keyType) {
                    case KeyType.Const: {
                            switch (propertyType) {
                                case PropertyType.AngleX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).AngleX = curve.Values[0];
                                    }

                                    break;
                                case PropertyType.AngleY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).AngleY = curve.Values[0];
                                    }

                                    break;
                                case PropertyType.AngleZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).AngleZ = curve.Values[0];
                                    }

                                    break;
                                case PropertyType.PositionX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).PositionX = curve.Values[0];
                                    }

                                    break;
                                case PropertyType.PositionY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).PositionY = curve.Values[0];
                                    }

                                    break;
                                case PropertyType.PositionZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        GetOrAddFrame(frameIndex, curve.Path).PositionZ = curve.Values[0];
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                            }
                        }
                        break;
                    case KeyType.FullFrame: {
                            var valueCount = curve.Values.Length;

                            switch (propertyType) {
                                case PropertyType.AngleX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).AngleX = curve.Values[index];
                                    }

                                    break;
                                case PropertyType.AngleY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).AngleY = curve.Values[index];
                                    }

                                    break;
                                case PropertyType.AngleZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).AngleZ = curve.Values[index];
                                    }

                                    break;
                                case PropertyType.PositionX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).PositionX = curve.Values[index];
                                    }

                                    break;
                                case PropertyType.PositionY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).PositionY = curve.Values[index];
                                    }

                                    break;
                                case PropertyType.PositionZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var index = frameIndex < valueCount ? frameIndex : valueCount - 1;
                                        GetOrAddFrame(frameIndex, curve.Path).PositionZ = curve.Values[index];
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                            }
                        }

                        break;
                    case KeyType.Discrete: {
                            if ((curve.Values.Length % 2) != 0) {
                                throw new ApplicationException($"Length of curve values {curve.Values.Length} is not a muliple of 2.");
                            }

                            var curveValueCount = curve.Values.Length / 2;
                            var curTime = TimeSpan.FromSeconds(curve.Values[0]);
                            var curValue = curve.Values[1];
                            var nextTime = TimeSpan.FromSeconds(curve.Values[2]);
                            var nextValue = curve.Values[3];
                            var curIndex = 0;

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
                                        nextTime = TimeSpan.FromSeconds(curve.Values[(curIndex + 1) * 2]);
                                        nextValue = curve.Values[(curIndex + 1) * 2 + 1];
                                    }
                                }

                                if (curIndex >= curveValueCount - 1) {
                                    return curValue;
                                }

                                var duration = (float)(nextTime - curTime).TotalSeconds;
                                var delta = (float)(frameTime - curTime).TotalSeconds;
                                var p = delta / duration;

                                return curValue * (1 - p) + nextValue * p;
                            }

                            switch (propertyType) {
                                case PropertyType.AngleX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.AngleX = value;
                                    }

                                    break;
                                case PropertyType.AngleY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.AngleY = value;
                                    }

                                    break;
                                case PropertyType.AngleZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.AngleZ = value;
                                    }

                                    break;
                                case PropertyType.PositionX:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.PositionX = value;
                                    }

                                    break;
                                case PropertyType.PositionY:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.PositionY = value;
                                    }

                                    break;
                                case PropertyType.PositionZ:
                                    for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
                                        var frame = GetOrAddFrame(frameIndex, curve.Path);
                                        var value = InterpolateValue(frame);
                                        frame.PositionZ = value;
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, $"Unknown property type: \"{propertyType}\".");
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(keyType), keyType, $"Unknown key type: \"{keyType}\".");
                }
            }

            var totalList = frameDict.SelectMany(kv => kv.Value).ToList();

            totalList.Sort((f1, f2) => {
                var s1 = f1.FrameIndex.CompareTo(f2.FrameIndex);

                if (s1 != 0) {
                    return s1;
                }

                return string.Compare(f1.Path, f2.Path, StringComparison.Ordinal);
            });

            return new Animation(totalList.ToArray(), TimeSpan.FromSeconds(dance.Duration));
        }

    }
}
