using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Mvd;
using OpenMLTD.MillionDance.Extensions;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class MvdCreator {

        [NotNull]
        public MvdMotion CreateCameraMotion([CanBeNull] CharacterImasMotionAsset mainCamera, [NotNull] ScenarioObject baseScenario, [CanBeNull] CharacterImasMotionAsset cameraAppeal, AppealType appealType) {
            MvdCameraMotion[] cameraFrames;

            if (ProcessCameraFrames && mainCamera != null) {
                cameraFrames = CreateCameraMotions(mainCamera, baseScenario, cameraAppeal, appealType);
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
        private MvdCameraMotion[] CreateCameraMotions([NotNull] CharacterImasMotionAsset mainCamera, [NotNull] ScenarioObject baseScenario, [CanBeNull] CharacterImasMotionAsset cameraAppeal, AppealType appealType) {
            var camera = CreateCamera();
            var cameraFrames = CreateFrames(mainCamera, baseScenario, cameraAppeal, appealType);
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
        private MvdCameraFrame[] CreateFrames([NotNull] CharacterImasMotionAsset mainCamera, [NotNull] ScenarioObject baseScenario, [CanBeNull] CharacterImasMotionAsset cameraAppeal, AppealType appealType) {
            var mainAnimation = CameraAnimation.CreateFrom(mainCamera);
            var appealAnimation = cameraAppeal != null ? CameraAnimation.CreateFrom(cameraAppeal) : null;
            var animationFrameCount = mainAnimation.CameraFrames.Length;

            var cameraFrameList = new List<MvdCameraFrame>();

            var appealTimes = appealType != AppealType.None ? AppealHelper.CollectAppealTimeInfo(baseScenario) : default;

            var transform60FpsTo30Fps = _conversionConfig.Transform60FpsTo30Fps;
            var scaleToVmdSize = _conversionConfig.ScaleToVmdSize;
            var unityToVmdScale = _scalingConfig.ScaleUnityToVmd;

            for (var mltdFrameIndex = 0; mltdFrameIndex < animationFrameCount; ++mltdFrameIndex) {
                if (transform60FpsTo30Fps) {
                    if (mltdFrameIndex % 2 == 1) {
                        continue;
                    }
                }

                // When entering and leaving the appeal, there is also a camera control event (type 58) with `layer` > 0 (see CollectFormationChanges() for the meaning of `layer`).
                var shouldUseAppeal = appealType != AppealType.None && (appealTimes.StartFrame <= mltdFrameIndex && mltdFrameIndex < appealTimes.EndFrame) && appealAnimation != null;
                var animation = shouldUseAppeal ? appealAnimation : mainAnimation;

                int projectedFrameIndex;

                if (shouldUseAppeal) {
                    var indexInAppeal = mltdFrameIndex - appealTimes.StartFrame;

                    if (indexInAppeal >= appealAnimation.FrameCount) {
                        indexInAppeal = appealAnimation.FrameCount - 1;
                    }

                    // `indexInAppeal`, unlike `mltdFrameIndex`, has not been scaled yet
                    if (transform60FpsTo30Fps) {
                        projectedFrameIndex = indexInAppeal / 2;
                    } else {
                        projectedFrameIndex = indexInAppeal;
                    }
                } else {
                    projectedFrameIndex = mltdFrameIndex;
                }

                var motionFrame = animation.CameraFrames[projectedFrameIndex];

                int mvdFrameIndex;

                if (transform60FpsTo30Fps) {
                    mvdFrameIndex = mltdFrameIndex / 2;
                } else {
                    mvdFrameIndex = mltdFrameIndex;
                }

                var mvdFrame = new MvdCameraFrame(mvdFrameIndex);

                var position = new Vector3(motionFrame.PositionX, motionFrame.PositionY, motionFrame.PositionZ);

                position = position.FixUnityToMmd();

                if (scaleToVmdSize) {
                    position = position * unityToVmdScale;
                }

                mvdFrame.Position = position;

                var target = new Vector3(motionFrame.TargetX, motionFrame.TargetY, motionFrame.TargetZ);

                target = target.FixUnityToMmd();

                if (scaleToVmdSize) {
                    target = target * unityToVmdScale;
                }

                var delta = target - position;

                mvdFrame.Distance = delta.Length;

                var q = CameraOrientation.QuaternionLookAt(in position, in target, in Vector3.UnitY);

                var rotation = CameraOrientation.ComputeMmdOrientation(in q, motionFrame.AngleZ);

                mvdFrame.Rotation = rotation;

                // MVD has good support of dynamic FOV. So here we can animate its value.
                var fov = FocalLengthToFov(motionFrame.FocalLength);
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
