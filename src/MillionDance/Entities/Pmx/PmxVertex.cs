using System;
using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxVertex : ICloneable {

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

        [NotNull]
        public PmxVertex Clone() {
            var result = new PmxVertex();

            result.Position = Position;
            result.Normal = Normal;
            result.UV = UV;

            for (var i = 0; i < MaxUvaCount; i += 1) {
                result.Uva[i] = Uva[i];
            }

            result.Deformation = Deformation;

            for (var i = 0; i < BoneWeights.Length; i += 1) {
                result.BoneWeights[i] = BoneWeights[i].Clone();
            }

            result.EdgeScale = EdgeScale;
            result.C0 = C0;
            result.R0 = R0;
            result.R1 = R1;
            result.RW0 = RW0;
            result.RW1 = RW1;

            return result;
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public const int MaxUvaCount = 4;

        public const int MaxBoneWeightCount = 4;

    }
}
