using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxVertex {

        internal PmxVertex() {
            var boneWeights = new BoneWeight[MaxBoneWeightCount];

            for (var i = 0; i < boneWeights.Length; ++i) {
                boneWeights[i] = new BoneWeight();
            }

            BoneWeights = boneWeights;
        }

        public Vector3 Position { get; internal set; }

        public Vector3 Normal { get; internal set; }

        public Vector2 UV { get; internal set; }

        /// <summary>
        /// UV Additions.
        /// </summary>
        [NotNull]
        public Vector4[] Uva { get; } = new Vector4[MaxUvaCount];

        public Deformation Deformation { get; internal set; }

        [NotNull, ItemNotNull]
        public BoneWeight[] BoneWeights { get; }

        public float EdgeScale { get; internal set; }

        public Vector3 C0 { get; internal set; }

        public Vector3 R0 { get; internal set; }

        public Vector3 R1 { get; internal set; }

        public Vector3 RW0 { get; internal set; }

        public Vector3 RW1 { get; internal set; }

        public const int MaxUvaCount = 4;

        public const int MaxBoneWeightCount = 4;

    }
}
