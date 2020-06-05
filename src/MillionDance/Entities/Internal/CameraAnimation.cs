using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Entities.Extensions;

namespace OpenMLTD.MillionDance.Entities.Internal {
    public sealed class CameraAnimation {

        private CameraAnimation([NotNull, ItemNotNull] CameraFrame[] frames, float duration) {
            CameraFrames = frames;
            Duration = duration;
        }

        [NotNull, ItemNotNull]
        public CameraFrame[] CameraFrames { get; }

        public float Duration { get; }

        [NotNull]
        public static CameraAnimation CreateFrom([NotNull] CharacterImasMotionAsset cameraMotion) {
            var focalLengthCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBase" && curve.GetPropertyName() == "focalLength");
            var camCutCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBase" && curve.GetPropertyName() == "camCut");
            var angleXCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.AngleX);
            var angleYCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.AngleY);
            var angleZCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.AngleZ);
            var posXCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.PositionX);
            var posYCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.PositionY);
            var posZCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamBaseS" && curve.GetPropertyType() == PropertyType.PositionZ);
            var targetXCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamTgtS" && curve.GetPropertyType() == PropertyType.PositionX);
            var targetYCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamTgtS" && curve.GetPropertyType() == PropertyType.PositionY);
            var targetZCurve = cameraMotion.Curves.FirstOrDefault(curve => curve.Path == "CamTgtS" && curve.GetPropertyType() == PropertyType.PositionZ);

            var allCameraCurves = new[] {
                focalLengthCurve, camCutCurve, angleXCurve, angleYCurve, angleZCurve, posXCurve, posYCurve, posZCurve, targetXCurve, targetYCurve, targetZCurve
            };

            if (AnyoneIsNull(allCameraCurves)) {
                throw new ApplicationException("Invalid camera motion file.");
            }

            Debug.Assert(focalLengthCurve != null, nameof(focalLengthCurve) + " != null");
            Debug.Assert(camCutCurve != null, nameof(camCutCurve) + " != null");
            Debug.Assert(angleXCurve != null, nameof(angleXCurve) + " != null");
            Debug.Assert(angleYCurve != null, nameof(angleYCurve) + " != null");
            Debug.Assert(angleZCurve != null, nameof(angleZCurve) + " != null");
            Debug.Assert(posXCurve != null, nameof(posXCurve) + " != null");
            Debug.Assert(posYCurve != null, nameof(posYCurve) + " != null");
            Debug.Assert(posZCurve != null, nameof(posZCurve) + " != null");
            Debug.Assert(targetXCurve != null, nameof(targetXCurve) + " != null");
            Debug.Assert(targetYCurve != null, nameof(targetYCurve) + " != null");
            Debug.Assert(targetZCurve != null, nameof(targetZCurve) + " != null");

            if (!AllUseFCurve(allCameraCurves)) {
                throw new ApplicationException("Invalid key type.");
            }

            const float frameDuration = 1.0f / FrameRate.Mltd;
            var totalDuration = GetMaxDuration(allCameraCurves);
            var frameCount = (int)Math.Round(totalDuration / frameDuration);

            var cameraFrames = new CameraFrame[frameCount];

            for (var i = 0; i < frameCount; ++i) {
                var frame = new CameraFrame();
                var time = i * frameDuration;

                frame.Time = time;
                frame.FocalLength = GetInterpolatedValue(focalLengthCurve, time);
                frame.Cut = (int)GetLowerClampedValue(camCutCurve, time);
                frame.AngleX = GetInterpolatedValue(angleXCurve, time);
                frame.AngleY = GetInterpolatedValue(angleYCurve, time);
                frame.AngleZ = GetInterpolatedValue(angleZCurve, time);
                frame.PositionX = GetInterpolatedValue(posXCurve, time);
                frame.PositionY = GetInterpolatedValue(posYCurve, time);
                frame.PositionZ = GetInterpolatedValue(posZCurve, time);
                frame.TargetX = GetInterpolatedValue(targetXCurve, time);
                frame.TargetY = GetInterpolatedValue(targetYCurve, time);
                frame.TargetZ = GetInterpolatedValue(targetZCurve, time);

                cameraFrames[i] = frame;
            }

            return new CameraAnimation(cameraFrames, totalDuration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AnyoneIsNull<T>([NotNull, ItemCanBeNull] params T[] objects) {
            return objects.Any(x => ReferenceEquals(x, null));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AllUseFCurve([NotNull, ItemNotNull] params Curve[] curves) {
            return curves.All(curve => curve.GetKeyType() == KeyType.FCurve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetMaxDuration([NotNull, ItemNotNull] params Curve[] curves) {
            float duration = 0;

            foreach (var curve in curves) {
                Debug.Assert(curve.Values.Length % 4 == 0);

                var frameCount = curve.Values.Length / 4;

                for (var i = 0; i < frameCount; ++i) {
                    var time = curve.Values[i * 4];

                    if (time > duration) {
                        duration = time;
                    }
                }
            }

            return duration;
        }

        private static float GetInterpolatedValue([NotNull] Curve curve, float time) {
            var valueCount = curve.Values.Length;

            Debug.Assert(valueCount % 4 == 0);

            valueCount = valueCount / 4;

            for (var i = 0; i < valueCount; ++i) {
                if (i < valueCount - 1) {
                    var nextTime = curve.Values[(i + 1) * 4];

                    if (time > nextTime) {
                        continue;
                    }

                    var curTime = curve.Values[i * 4];
                    var curValue = curve.Values[i * 4 + 1];
                    var nextValue = curve.Values[(i + 1) * 4 + 1];
                    var tan1 = curve.Values[i * 4 + 3];
                    var tan2 = curve.Values[(i + 1) * 4 + 2];

                    // suspect:
                    // +2: tan(in)
                    // +3: tan(out)

                    var dt = nextTime - curTime;
                    var t = (time - curTime) / dt;

                    // TODO: use F-curve interpolation.
                    //return Lerp(curValue, nextValue, t);
                    return ComputeFCurveNaive(curValue, nextValue, tan1, tan2, dt, t);
                } else {
                    return curve.Values[i * 4 + 1];
                }
            }

            throw new ArgumentException("Maybe time is invalid.");
        }

        private static float GetLowerClampedValue([NotNull] Curve curve, float time) {
            var valueCount = curve.Values.Length;

            Debug.Assert(valueCount % 4 == 0);

            valueCount = valueCount / 4;

            for (var i = 0; i < valueCount; ++i) {
                if (i < valueCount - 1) {
                    var nextTime = curve.Values[(i + 1) * 4];

                    if (time > nextTime) {
                        continue;
                    }

                    var curValue = curve.Values[i * 4 + 1];

                    return curValue;
                } else {
                    return curve.Values[i * 4 + 1];
                }
            }

            throw new ArgumentException("Maybe time is invalid.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Lerp(float from, float to, float t) {
            return from * (1 - t) + to * t;
        }

        // https://developer.blender.org/diffusion/B/browse/master/source/blender/blenkernel/intern/fcurve.c;e50a3dd4c4e9a9898df31e444d1002770b4efb9c$2212

        // Key: some mathematics
        // See http://luthuli.cs.uiuc.edu/~daf/courses/cs-419/Week-12/Interpolation-2013.pdf pg. 26
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ComputeFCurveNaive(float value1, float value2, float tan1, float tan2, float dt, float t) {
            bool isInf1 = float.IsInfinity(tan1), isInf2 = float.IsInfinity(tan2);

            //float factor = Math.Abs(value2 - value1);
            //float factor = value2 - value1;
            //float factor = dt;
            //float factor = 1;
            //float factor = 2; // funny effect when playing Blooming Star at "きらめきに憧れて　胸で"
            //float factor = 3; // too large

            // There should be another parameter to specify the exact location tuple (time, value) is,
            // for example handle length. From differentiation we know the tangents of the control
            // points. So here variable "factor" means a mutiplication factor, which is similar to
            // handle length. We use this to compute the location of one or two control points.
            // For F-curve editing (and why I guess like above), see Blender's manual:
            // https://docs.blender.org/manual/en/dev/editors/graph_editor/fcurves/introduction.html
            float factor = dt;

            if (isInf1) {
                if (isInf2) {
                    return Lerp(value1, value2, t);
                } else {
                    var cp = value2 - tan2 / 3 * factor;
                    return Bezier(value1, cp, value2, t);
                }
            } else {
                if (isInf2) {
                    var cp = value1 + tan1 / 3 * factor;
                    return Bezier(value1, cp, value2, t);
                } else {
                    var cp1 = value1 + tan1 / 3 * factor;
                    var cp2 = value2 - tan2 / 3 * factor;
                    return Bezier(value1, cp1, cp2, value2, t);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Bezier(float p1, float cp, float p2, float t) {
            return (1 - t) * (1 - t) * p1 + 2 * t * (1 - t) * cp + t * t * p2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Bezier(float p1, float cp1, float cp2, float p2, float t) {
            var tt = 1 - t;
            var tt2 = tt * tt;
            var t2 = t * t;

            return tt * tt2 * p1 + 3 * tt2 * t * cp1 + 3 * tt * t2 * cp2 + t * t2 * p2;
        }

    }
}
