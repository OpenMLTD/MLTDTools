using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Mvd;
using MillionDance.Extensions;
using OpenTK;

namespace MillionDance.Core {
    partial class MvdCreator {

        [NotNull, ItemNotNull]
        private static IReadOnlyList<MvdCameraMotion> CreateCameraMotions([NotNull] CharacterImasMotionAsset cameraMotion) {
            const string modelName = "MODEL_00";
            const string cameraName = "カメラ00"; // Localization error (Mr. mogg, please!), MUST NOT USE "Camera00"!

            MvdCameraObject CreateCamera() {
                var cam = new MvdCameraObject();

                cam.DisplayName = cameraName;
                cam.EnglishName = cameraName;

                cam.Id = 0;

                return cam;
            }

            IReadOnlyList<MvdCameraFrame> CreateFrames() {
                var animation = CameraAnimation.CreateFrom(cameraMotion);
                var animationFrameCount = animation.CameraFrames.Count;

                var cameraFrameList = new List<MvdCameraFrame>();

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
                    var mvdFrame = new MvdCameraFrame(frameIndex);

                    var pos = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                    pos = pos.FixUnityToOpenTK();

                    if (ConversionConfig.Current.ScaleToVmdSize) {
                        pos = pos * ScalingConfig.ScaleUnityToMmd;
                    }

                    mvdFrame.Position = pos;

                    var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                    target = target.FixUnityToOpenTK();

                    if (ConversionConfig.Current.ScaleToVmdSize) {
                        target = target * ScalingConfig.ScaleUnityToMmd;
                    }

                    var delta = target - pos;

                    mvdFrame.Distance = delta.Length;

                    var qy = MathHelper.Pi - (float)Math.Atan2(delta.X, delta.Z);
                    var qx = -(float)Math.Atan2(delta.Y, delta.Z);
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

                    mvdFrame.Rotation = new Vector3(qx, qy, qz);

                    // VMD does not have good support for animated FOV. So here just use a constant to avoid "jittering".
                    // The drawback is, some effects (like the first zooming cut in Shooting Stars) will not be able to achieve.
                    var fov = FocalLengthToFov(frame.FocalLength);
                    mvdFrame.FieldOfView = MathHelper.DegreesToRadians(fov);

                    cameraFrameList.Add(mvdFrame);
                }

                return cameraFrameList;
            }

            IReadOnlyList<MvdCameraPropertyFrame> CreatePropertyFrames() {
                var list = new List<MvdCameraPropertyFrame>();

                var f = new MvdCameraPropertyFrame(0);

                f.Enabled = true;
                f.IsPerspective = true;
                f.Alpha = 1;
                f.EffectEnabled = true;
                f.DynamicFovEnabled = false;
                f.DynamicFovRate = 0.1f;
                f.DynamicFovCoefficient = 1;
                f.RelatedBoneId = -1;
                f.RelatedModelId = -1;

                list.Add(f);

                return list;
            }

            // https://photo.stackexchange.com/questions/41273/how-to-calculate-the-fov-in-degrees-from-focal-length-or-distance
            float FocalLengthToFov(float focalLength) {
                // By experiments, MLTD seems to use a 15mm or 16mm camera.
                const float sensorSize = 15; // unit: mm, as the unit of MLTD camera frame is also mm
                var fovRad = 2 * (float)Math.Atan((sensorSize / 2) / focalLength);
                var fovDeg = MathHelper.RadiansToDegrees(fovRad);

                return fovDeg;
            }

            var camera = CreateCamera();
            var cameraFrames = CreateFrames();
            var cameraPropertyFrames = CreatePropertyFrames();

            var result = new MvdCameraMotion(camera, cameraFrames, cameraPropertyFrames);

            result.DisplayName = cameraName;
            result.EnglishName = cameraName;

            return new[] { result };
        }

    }
}
