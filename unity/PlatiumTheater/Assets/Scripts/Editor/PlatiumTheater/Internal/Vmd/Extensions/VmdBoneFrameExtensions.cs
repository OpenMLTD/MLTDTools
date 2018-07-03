using System;
using JetBrains.Annotations;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace PlatiumTheater.Internal.Vmd.Extensions {
    public static class VmdBoneFrameExtensions {

        public static VmdBoneFrame CopyWithDifferentFrameIndex([NotNull] this VmdBoneFrame thisFrame, int newFrameIndex) {
            var frame = new VmdBoneFrame();

            frame.Name = thisFrame.Name;
            frame.FrameIndex = newFrameIndex;
            Buffer.BlockCopy(thisFrame.Interpolation, 0, frame.Interpolation, 0, thisFrame.Interpolation.Length);

            frame.Position = thisFrame.Position;
            frame.Rotation = thisFrame.Rotation;

            return frame;
        }

        public static VmdBoneFrame LerpTo([NotNull] this VmdBoneFrame thisFrame, [NotNull] VmdBoneFrame nextFrame, float t, float frameRateRatio = 1) {
            Debug.Assert(thisFrame.Name == nextFrame.Name, "thisFrame.Name == nextFrame.Name");

            t = Mathf.Clamp(t, 0, 1);

            var frame = new VmdBoneFrame();

            frame.Name = thisFrame.Name;
            frame.FrameIndex = (int)(Mathf.Lerp(thisFrame.FrameIndex, nextFrame.FrameIndex, t) * frameRateRatio);
            Buffer.BlockCopy(thisFrame.Interpolation, 0, frame.Interpolation, 0, thisFrame.Interpolation.Length);

            frame.Position = Vector3.Lerp(thisFrame.Position, nextFrame.Position, t);
            frame.Rotation = Quaternion.Slerp(thisFrame.Rotation, nextFrame.Rotation, t);

            return frame;
        }

    }
}
