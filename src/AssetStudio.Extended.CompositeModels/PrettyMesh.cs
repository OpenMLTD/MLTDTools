using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public abstract class PrettyMesh {

        private protected PrettyMesh() {
        }

        [NotNull]
        public abstract string Name { get; }

        [NotNull]
        public abstract PrettySubMesh[] SubMeshes { get; }

        [NotNull]
        public abstract uint[] Indices { get; }

        [NotNull, ItemNotNull]
        public abstract BoneInfluence[][] Skin { get; }

        [NotNull]
        public abstract Matrix4x4[] BindPose { get; }

        public abstract int VertexCount { get; }

        [NotNull]
        public abstract Vector3[] Vertices { get; }

        [NotNull]
        public abstract Vector3[] Normals { get; }

        [CanBeNull]
        public abstract Vector4[] Colors { get; }

        [NotNull]
        public abstract Vector2[] UV1 { get; }

        [NotNull, ItemCanBeNull]
        public abstract Vector2?[] UV2 { get; }

        [CanBeNull]
        public abstract Vector3[] Tangents { get; }

        [NotNull]
        public abstract uint[] BoneNameHashes { get; }

        [CanBeNull]
        public abstract BlendShapeData Shape { get; }

    }
}
