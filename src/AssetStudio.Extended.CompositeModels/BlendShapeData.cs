using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class BlendShapeData {

        internal BlendShapeData([NotNull, ItemNotNull] BlendShapeVertex[] vertices, [NotNull, ItemNotNull] MeshBlendShape[] shapes, [NotNull, ItemNotNull] MeshBlendShapeChannel[] channels, [NotNull] float[] fullWeights) {
            Vertices = vertices;
            Shapes = shapes;
            Channels = channels;
            FullWeights = fullWeights;
        }

        internal BlendShapeData([NotNull] AssetStudio.BlendShapeData data) {
            {
                var vertexCount = data.vertices.Length;
                var vertices = new BlendShapeVertex[vertexCount];

                for (var i = 0; i < vertexCount; i += 1) {
                    vertices[i] = new BlendShapeVertex(data.vertices[i]);
                }

                Vertices = vertices;
            }

            {
                var shapeCount = data.shapes.Length;
                var shapes = new MeshBlendShape[shapeCount];

                for (var i = 0; i < shapeCount; i += 1) {
                    shapes[i] = new MeshBlendShape(data.shapes[i]);
                }

                Shapes = shapes;
            }

            {
                var channelCount = data.channels.Length;
                var channels = new MeshBlendShapeChannel[channelCount];

                for (var i = 0; i < channelCount; i += 1) {
                    channels[i] = new MeshBlendShapeChannel(data.channels[i]);
                }

                Channels = channels;
            }

            FullWeights = (float[])data.fullWeights.Clone();
        }

        [NotNull, ItemNotNull]
        public BlendShapeVertex[] Vertices { get; }

        [NotNull, ItemNotNull]
        public MeshBlendShape[] Shapes { get; }

        [NotNull, ItemNotNull]
        public MeshBlendShapeChannel[] Channels { get; }

        [NotNull]
        public float[] FullWeights { get; }

    }
}
