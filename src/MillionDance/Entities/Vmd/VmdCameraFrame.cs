using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Entities.Vmd {
    public sealed class VmdCameraFrame : VmdBaseFrame {

        internal VmdCameraFrame(int frameIndex)
            : base(frameIndex) {
            Interpolation = new byte[6, 4];
            Unknown = new byte[3];
        }

        public float Distance { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 Orientation { get; internal set; }

        [NotNull]
        public byte[,] Interpolation { get; }

        public float FieldOfView { get; internal set; }

        [NotNull]
        public byte[] Unknown { get; }

    }
}
