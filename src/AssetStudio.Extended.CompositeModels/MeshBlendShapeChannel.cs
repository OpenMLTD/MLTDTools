using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class MeshBlendShapeChannel {

        internal MeshBlendShapeChannel([NotNull] string name, uint nameHash, int frameIndex, int frameCount) {
            Name = name;
            NameHash = nameHash;
            FrameIndex = frameIndex;
            FrameCount = frameCount;
        }

        internal MeshBlendShapeChannel([NotNull] AssetStudio.MeshBlendShapeChannel channel) {
            Name = channel.name;
            NameHash = channel.nameHash;
            FrameIndex = channel.frameIndex;
            FrameCount = channel.frameCount;
        }

        [NotNull]
        public string Name { get; }

        public uint NameHash { get; }

        public int FrameIndex { get; }

        public int FrameCount { get; }

    }
}
