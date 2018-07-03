using JetBrains.Annotations;
using UnityEngine;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdBoneFrame : VmdBaseFrame {

        internal VmdBoneFrame() {
            Interpolation = new byte[4, 4, 4];
        }

        [NotNull]
        public string Name { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Quaternion Rotation { get; internal set; }

        [NotNull]
        public byte[,,] Interpolation { get; private set; }

        public override string ToString() {
            return string.Format("BoneFrame \"{0}\" FrameIndex={1} (Position: {2}; Rotation: {3})", Name, FrameIndex, Position, Rotation);
        }

    }
}
