using System;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class BoneWeight : ICloneable {

        internal BoneWeight() {
        }

        public int BoneIndex { get; internal set; } = PmxBone.InvalidBoneIndex;

        public float Weight { get; internal set; }

        [NotNull]
        public BoneWeight Clone() {
            var result = new BoneWeight();

            result.BoneIndex = BoneIndex;
            result.Weight = Weight;

            return result;
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public bool IsValid => BoneIndex != PmxBone.InvalidBoneIndex && !Weight.Equals(0);

    }
}
