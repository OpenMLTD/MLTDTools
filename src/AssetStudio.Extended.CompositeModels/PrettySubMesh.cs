using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class PrettySubMesh {

        internal PrettySubMesh(uint firstIndex, uint indexCount, PrimitiveType topology, uint triangleCount, uint firstVertex, uint vertexCount, AABB boundingBox, [NotNull] TexturedMaterial material) {
            FirstIndex = firstIndex;
            IndexCount = indexCount;
            Topology = topology;
            TriangleCount = triangleCount;
            FirstVertex = firstVertex;
            VertexCount = vertexCount;
            BoundingBox = boundingBox;
            Material = material;
        }

        internal PrettySubMesh(uint firstIndex, [NotNull] AssetStudio.SubMesh mesh, [NotNull] TexturedMaterial material) {
            FirstIndex = firstIndex;
            IndexCount = mesh.indexCount;
            Topology = (PrimitiveType)mesh.topology;
            TriangleCount = mesh.triangleCount;
            FirstVertex = mesh.firstVertex;
            VertexCount = mesh.vertexCount;
            BoundingBox = new AABB(mesh.localAABB);
            Material = material;
        }

        public uint FirstIndex { get; }

        public uint IndexCount { get; }

        public PrimitiveType Topology { get; }

        public uint TriangleCount { get; }

        public uint FirstVertex { get; }

        public uint VertexCount { get; }

        public AABB BoundingBox { get; }

        [NotNull]
        public TexturedMaterial Material { get; }

    }
}
