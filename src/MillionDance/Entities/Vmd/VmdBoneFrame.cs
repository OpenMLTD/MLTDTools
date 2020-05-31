using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdBoneFrame : VmdBaseFrame {

        static VmdBoneFrame() {
            SharedInterpolation = CreateDefaultInterpolation();
        }

        internal VmdBoneFrame(int frameIndex, [NotNull] string name)
            : this(frameIndex, name, true) {
        }

        internal VmdBoneFrame(int frameIndex, [NotNull] string name, bool useSharedInterpolation)
            : base(frameIndex) {
            Name = name;

            if (useSharedInterpolation) {
                // Usually we don't care about it anyway.
                // TODO: Use copy-on-write technique or declare it as immutable.
                Interpolation = SharedInterpolation;
            } else {
                Interpolation = CreateDefaultInterpolation();
            }
        }

        [NotNull]
        public string Name { get; }

        public Vector3 Position { get; internal set; } = Vector3.Zero;

        public Quaternion Rotation { get; internal set; } = Quaternion.Identity;

        [NotNull]
        public byte[,,] Interpolation { get; }

        public override string ToString() {
            return $"BoneFrame \"{Name}\" FrameIndex={FrameIndex} (Position: {Position}; Rotation: {Rotation})";
        }

        [NotNull]
        private static byte[,,] CreateDefaultInterpolation() {
            var interpolation = new byte[4, 4, 4];

            // Default interpolation (linear)
            for (var i = 0; i < 4; ++i) {
                for (var j = 0; j < 4; ++j) {
                    for (var k = 0; k < 4; ++k) {
                        interpolation[i, j, k] = 127;
                    }
                }
            }

            return interpolation;
        }

        [NotNull]
        private static readonly byte[,,] SharedInterpolation;

    }
}
