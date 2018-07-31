using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MillionDance.Entities.Extensions;
using MillionDance.Entities.Mltd;

namespace MillionDance.Entities.Internal {
    public sealed class CameraAnimation {

        private CameraAnimation([NotNull, ItemNotNull] CameraFrame[] frames, float duration) {
            CameraFrames = frames;
            Duration = duration;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<CameraFrame> CameraFrames { get; }

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

            const float mltdFps = 60;
            const float frameDuration = 1 / mltdFps;
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

                    // TODO: use F-curve interpolation.
                    return Lerp(curValue, nextValue, (time - curTime) / (nextTime - curTime));
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

    }
}
