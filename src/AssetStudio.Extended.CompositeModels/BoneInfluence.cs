using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class BoneInfluence {

        internal BoneInfluence(int boneIndex, float weight) {
            BoneIndex = boneIndex;
            Weight = weight;
        }

        internal BoneInfluence([NotNull] BoneWeights4 weights, int index) {
            BoneIndex = weights.boneIndex[index];
            Weight = weights.weight[index];
        }

        public int BoneIndex { get; }

        public float Weight { get; }

    }
}
