using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class BlendShapeVertex {

        internal BlendShapeVertex(Vector3 vertex, Vector3 normal, Vector3 tangent, uint index) {
            Vertex = vertex;
            Normal = normal;
            Tangent = tangent;
            Index = index;
        }

        internal BlendShapeVertex([NotNull] AssetStudio.BlendShapeVertex vertex) {
            Vertex = vertex.vertex;
            Normal = vertex.normal;
            Tangent = vertex.tangent;
            Index = vertex.index;
        }

        public Vector3 Vertex { get; }

        public Vector3 Normal { get; }

        public Vector3 Tangent { get; }

        public uint Index { get; }

    }
}
