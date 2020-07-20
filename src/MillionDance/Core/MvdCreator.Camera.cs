using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            MvdCameraMotion[] cameraFrames;

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
        private MvdCameraMotion[] CreateCameraMotions([NotNull] CharacterImasMotionAsset cameraMotion) {
            var camera = CreateCamera();
            var cameraFrames = CreateFrames(cameraMotion);
            var cameraPropertyFrames = CreatePropertyFrames();

            var result = new MvdCameraMotion(camera, cameraFrames, cameraPropertyFrames);

            result.DisplayName = CameraName;
            result.EnglishName = CameraName;

            return new[] { result };
        }

        private static MvdCameraObject CreateCamera() {
            var cam = new MvdCameraObject();

            cam.DisplayName = CameraName;
            cam.EnglishName = CameraName;

            cam.Id = 0;

            return cam;
        }

        [NotNull, ItemNotNull]
        private MvdCameraFrame[] CreateFrames([NotNull] CharacterImasMotionAsset cameraMotion) {
            var animation = CameraAnimation.CreateFrom(cameraMotion);
            var animationFrameCount = animation.CameraFrames.Length;

            var cameraFrameList = new List<MvdCameraFrame>();

            var scaleToVmdSize = _conversionConfig.ScaleToVmdSize;
            var unityToVmdScale = _scalingConfig.ScaleUnityToVmd;

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

                var position = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                position = position.FixUnityToMmd();

                if (scaleToVmdSize) {
                    position = position * unityToVmdScale;
                }

                mvdFrame.Position = position;

                var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                target = target.FixUnityToMmd();

                if (scaleToVmdSize) {
                    target = target * unityToVmdScale;
                }

                var delta = target - position;

                mvdFrame.Distance = delta.Length;

                var lookAtMatrix = Matrix4.LookAt(position, target, Vector3.UnitY);
                var q = lookAtMatrix.ExtractRotation();

                var rot = CameraOrientation.ComputeMmdOrientation(in q, frame.AngleZ);

                mvdFrame.Rotation = rot;

                // MVD has good support of dynamic FOV. So here we can animate its value.
                var fov = FocalLengthToFov(frame.FocalLength);
                mvdFrame.FieldOfView = MathHelper.DegreesToRadians(fov);

                cameraFrameList.Add(mvdFrame);
            }

            return cameraFrameList.ToArray();
        }

        [NotNull, ItemNotNull]
        private static MvdCameraPropertyFrame[] CreatePropertyFrames() {
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

            return list.ToArray();
        }

        // https://photo.stackexchange.com/questions/41273/how-to-calculate-the-fov-in-degrees-from-focal-length-or-distance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float FocalLengthToFov(float focalLength) {
            // MLTD uses physical camera
            // unit: mm, as the unit of MLTD camera frame is also mm
            const float sensorSizeY = 22.0f / 2;
            var fovRad = 2 * (float)Math.Atan((sensorSizeY / 2) / focalLength);
            var fovDeg = MathHelper.RadiansToDegrees(fovRad);

            return fovDeg;
        }

        private const string CameraName = "カメラ00"; // Localization error (Mr. mogg, please!), MUST NOT USE "Camera00"!

    }
}
