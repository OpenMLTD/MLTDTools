using System;
using System.Collections.Generic;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Mvd;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class MvdCreator {

        [NotNull]
        public MvdMotion CreateCameraMotion([CanBeNull] CharacterImasMotionAsset cameraMotion) {
            IReadOnlyList<MvdCameraMotion> cameraFrames;

            if (ProcessCameraFrames && cameraMotion != null) {
                cameraFrames = CreateCameraMotions(cameraMotion);
            } else {
                cameraFrames = Array.Empty<MvdCameraMotion>();
            }

            var mvd = new MvdMotion(cameraFrames);

            if (_conversionConfig.Transform60FpsTo30Fps) {
                mvd.Fps = FrameRate.Mmd;
            } else {
                mvd.Fps = FrameRate.Mltd;
            }

            return mvd;
        }

        [NotNull, ItemNotNull]
        private IReadOnlyList<MvdCameraMotion> CreateCameraMotions([NotNull] CharacterImasMotionAsset cameraMotion) {
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
                    if (_conversionConfig.Transform60FpsTo30Fps) {
                        if (i % 2 == 1) {
                            continue;
                        }
                    }

                    int frameIndex;

                    if (_conversionConfig.Transform60FpsTo30Fps) {
                        frameIndex = i / 2;
                    } else {
                        frameIndex = i;
                    }

                    var frame = animation.CameraFrames[i];
                    var mvdFrame = new MvdCameraFrame(frameIndex);

                    var pos = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                    pos = pos.FixUnityToOpenTK();

                    if (_conversionConfig.ScaleToVmdSize) {
                        pos = pos * _scalingConfig.ScaleUnityToVmd;
                    }

                    mvdFrame.Position = pos;

                    var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                    target = target.FixUnityToOpenTK();

                    if (_conversionConfig.ScaleToVmdSize) {
                        target = target * _scalingConfig.ScaleUnityToVmd;
                    }

                    var delta = target - pos;

                    mvdFrame.Distance = delta.Length;

                    var lookAtMatrix = Matrix4.LookAt(pos, target, Vector3.UnitY);
                    var q = lookAtMatrix.ExtractRotation();

                    var rot = q.DecomposeRad();

                    rot.Z = MathHelper.DegreesToRadians(frame.AngleZ);

                    rot.Y = MathHelper.Pi - rot.Y;

                    if (rot.Y < 0) {
                        rot.Y += MathHelper.TwoPi;
                    }

                    if (delta.Z < 0) {
                        rot.Y = -(MathHelper.Pi + rot.Y);
                    }

                    rot.Z = -rot.Z;

                    if (rot.X < -MathHelper.PiOver2) {
                        rot.X = rot.X + MathHelper.Pi;
                    } else if (rot.X > MathHelper.PiOver2) {
                        rot.X = -(MathHelper.Pi - rot.X);
                    }

                    mvdFrame.Rotation = rot;

                    // MVD has good support of dynamic FOV. So here we can animate its value.
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
                const float sensorSize = 12; // unit: mm, as the unit of MLTD camera frame is also mm
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
