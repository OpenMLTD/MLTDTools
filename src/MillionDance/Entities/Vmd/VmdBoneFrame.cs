using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Entities.Vmd {
    public sealed class VmdBoneFrame : VmdBaseFrame {

        internal VmdBoneFrame(int frameIndex, [NotNull] string name)
            : base(frameIndex) {
            Name = name;

            var interpolation = new byte[4, 4, 4];

            for (var i = 0; i < 4; ++i) {
                for (var j = 0; j < 4; ++j) {
                    for (var k = 0; k < 4; ++k) {
                        interpolation[i, j, k] = 127;
                    }
                }
            }

            Interpolation = interpolation;
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

    }
}
