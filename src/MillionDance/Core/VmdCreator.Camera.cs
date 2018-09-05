using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Vmd;
using MillionDance.Extensions;
using MillionDance.Utilities;
using OpenTK;

namespace MillionDance.Core {
    partial class VmdCreator {

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdCameraFrame> CreateCameraFrames([NotNull] CharacterImasMotionAsset cameraMotion, uint fixedFov) {
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
                    pos = pos * ScalingConfig.ScaleUnityToVmd;
                }

                vmdFrame.Position = pos;

                var target = new Vector3(frame.TargetX, frame.TargetY, frame.TargetZ);

                target = target.FixUnityToOpenTK();

                if (ConversionConfig.Current.ScaleToVmdSize) {
                    target = target * ScalingConfig.ScaleUnityToVmd;
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
