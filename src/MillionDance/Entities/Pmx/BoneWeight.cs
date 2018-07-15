namespace MillionDance.Entities.Pmx {
    public sealed class BoneWeight {

        internal BoneWeight() {
        }

        public int BoneIndex { get; internal set; } = PmxBone.InvalidBoneIndex;

        public float Weight { get; internal set; }

        public bool IsValid => BoneIndex != PmxBone.InvalidBoneIndex && !Weight.Equals(0);

    }
}
