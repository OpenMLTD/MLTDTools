using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class MeshBlendShape {

        internal MeshBlendShape(uint firstVertex, uint vertexCount, bool hasNormals, bool hasTangents) {
            FirstVertex = firstVertex;
            VertexCount = vertexCount;
            HasNormals = hasNormals;
            HasTangents = hasTangents;
        }

        internal MeshBlendShape([NotNull] AssetStudio.MeshBlendShape shape) {
            FirstVertex = shape.firstVertex;
            VertexCount = shape.vertexCount;
            HasNormals = shape.hasNormals;
            HasTangents = shape.hasTangents;
        }

        public uint FirstVertex { get; }

        public uint VertexCount { get; }

        public bool HasNormals { get; }

        public bool HasTangents { get; }

    }
}
