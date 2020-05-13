using System.Collections.Generic;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateCameraMotion([CanBeNull] CharacterImasMotionAsset cameraMotion) {
            IReadOnlyList<VmdCameraFrame> frames;

            if (ProcessCameraFrames && cameraMotion != null) {
                frames = CreateCameraFrames(cameraMotion, FixedFov);
            } else {
                frames = null;
            }

            return new VmdMotion(ModelName, null, null, frames, null, null);
        }

        [NotNull, ItemNotNull]
        private IReadOnlyList<VmdCameraFrame> CreateCameraFrames([NotNull] CharacterImasMotionAsset cameraMotion, uint fixedFov) {
            var animation = CameraAnimation.CreateFrom(cameraMotion);
            var animationFrameCount = animation.CameraFrames.Count;

            var cameraFrameList = new List<VmdCameraFrame>();

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
                var vmdFrame = new VmdCameraFrame(frameIndex);

                var pos = new Vector3(frame.PositionX, frame.PositionY, frame.PositionZ);

                pos = pos.FixUnityToOpenTK();

                if (_conversionConfig.ScaleToVmdSize) {
                    pos = pos * _scalingConfig.ScaleUnityToVmd;
                }

                vmdFrame.Position = pos;

                var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                target = target.FixUnityToOpenTK();

                if (_conversionConfig.ScaleToVmdSize) {
                    target = target * _scalingConfig.ScaleUnityToVmd;
                }

                var delta = target - pos;

                vmdFrame.Length = delta.Length;

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

                if (rot.X < -MathHelper.PiOver2) {
                    rot.X = rot.X + MathHelper.Pi;
                } else if (rot.X > MathHelper.PiOver2) {
                    rot.X = -(MathHelper.Pi - rot.X);
                }

                rot.X = -rot.X;
                rot.Y -= MathHelper.Pi;

                vmdFrame.Orientation = rot;

                // VMD does not have good support for animated FOV. So here just use a constant to avoid "jittering".
                // The drawback is, some effects (like the first zooming cut in Shooting Stars) will not be able to achieve.
                //var fov = FocalLengthToFov(frame.FocalLength);
                //vmdFrame.FieldOfView = (uint)fov;

                vmdFrame.FieldOfView = fixedFov;

                cameraFrameList.Add(vmdFrame);
            }

            return cameraFrameList;
        }

    }
}
