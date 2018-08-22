using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Vmd;
using MillionDance.Extensions;
using OpenTK;

namespace MillionDance.Core {
    partial class VmdCreator {

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdCameraFrame> CreateCameraFrames([NotNull] CharacterImasMotionAsset cameraMotion) {
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

                var qy = -(float)Math.Atan2(delta.X, delta.Z);
                var qx = (float)Math.Atan2(delta.Y, delta.Z);
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

                vmdFrame.Orientation = new Vector3(qx, qy, qz);

                // VMD does not have good support for animated FOV. So here just use a constant to avoid "jittering".
                // The drawback is, some effects (like the first zooming cut in Shooting Stars) will not be able to achieve.
                //var fov = FocalLengthToFov(frame.FocalLength);
                //vmdFrame.FieldOfView = (uint)fov;

                const uint constFov = 20;
                vmdFrame.FieldOfView = constFov;

                cameraFrameList.Add(vmdFrame);
            }

            return cameraFrameList;
        }

    }
}
