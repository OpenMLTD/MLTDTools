using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class CompositeMesh : PrettyMesh {

        private CompositeMesh([NotNull] PrettyMesh[] meshes) {
            ComponentMeshCount = meshes.Length;

            {
                var meshNames = new List<string>();

                foreach (var m in meshes) {
                    if (m is CompositeMesh cm) {
                        meshNames.AddRange(cm.Names);
                    } else {
                        meshNames.Add(m.Name);
                    }
                }

                Names = meshNames.ToArray();

                var sb = new StringBuilder();

                sb.Append("Composite mesh of:");

                foreach (var name in meshNames) {
                    sb.AppendLine();
                    sb.Append("\t");
                    sb.Append(name);
                }

                Name = sb.ToString();
            }

            var subMeshList = new List<PrettySubMesh>();
            var indexList = new List<uint>();
            var skinList = new List<BoneInfluence[]>();
            var bindPoseList = new List<Matrix4x4>();
            var vertexList = new List<Vector3>();
            var normalList = new List<Vector3>();
            var colorList = new List<Vector4>();
            var uv1List = new List<Vector2>();
            // uv2 is the optional secondary UV (highlight etc.)
            var uv2List = new List<Vector2?>();
            var tangentList = new List<Vector3>();
            var boneNameHashList = new List<uint>();
            var parentMeshIndices = new List<int>();

            uint vertexStart = 0;
            uint indexStart = 0;
            var boneHashStart = 0;
            var parentMeshIndex = 0;

            foreach (var mesh in meshes) {
                var cm = mesh as CompositeMesh;

                for (var i = 0; i < mesh.SubMeshes.Length; i++) {
                    var subMesh = mesh.SubMeshes[i];
                    Debug.Assert(subMesh.Topology == PrimitiveType.Triangles);

                    var newSubMesh = new PrettySubMesh(
                        subMesh.FirstIndex + indexStart, subMesh.IndexCount, subMesh.Topology, subMesh.TriangleCount,
                        subMesh.FirstVertex + vertexStart, subMesh.VertexCount, subMesh.BoundingBox, subMesh.Material);

                    subMeshList.Add(newSubMesh);

                    if (cm != null) {
                        parentMeshIndices.Add(parentMeshIndex + cm.ParentMeshIndices[i]);
                    } else {
                        parentMeshIndices.Add(parentMeshIndex);
                    }
                }

                if (cm != null) {
                    parentMeshIndex += cm.Names.Length;
                } else {
                    ++parentMeshIndex;
                }

                foreach (var index in mesh.Indices) {
                    indexList.Add(index + vertexStart);
                }

                foreach (var skin in mesh.Skin) {
                    var s = new BoneInfluence[4];

                    for (var i = 0; i < skin.Length; i++) {
                        var influence = skin[i];

                        if (influence == null) {
                            break;
                        }

                        var newInfluence = new BoneInfluence(influence.BoneIndex + boneHashStart, influence.Weight);

                        s[i] = newInfluence;
                    }

                    skinList.Add(s);
                }

                // Some vertex do not bind to a bone (e.g. 100 vertices & 90 bone influences),
                // so we have to fill the gap ourselves, or this will influence the meshes
                // combined after current mesh.
                if (mesh.Skin.Length < mesh.VertexCount) {
                    var difference = mesh.VertexCount - mesh.Skin.Length;

                    for (var i = 0; i < difference; ++i) {
                        skinList.Add(new BoneInfluence[4]);
                    }
                }

                boneNameHashList.AddRange(mesh.BoneNameHashes);

                bindPoseList.AddRange(mesh.BindPose);
                vertexList.AddRange(mesh.Vertices);
                normalList.AddRange(mesh.Normals);

                if (mesh.Colors != null) {
                    colorList.AddRange(mesh.Colors);
                } else {
                    for (var i = 0; i < mesh.VertexCount; ++i) {
                        colorList.Add(Vector4.Zero);
                    }
                }

                // TODO: counts don't match?
                if (mesh.Tangents != null) {
                    tangentList.AddRange(mesh.Tangents);
                } else {
                    for (var i = 0; i < mesh.VertexCount; ++i) {
                        tangentList.Add(Vector3.Zero);
                    }
                }

                uv1List.AddRange(mesh.UV1);
                uv2List.AddRange(mesh.UV2);

                vertexStart += (uint)mesh.VertexCount;
                indexStart += (uint)mesh.Indices.Length;
                boneHashStart += mesh.BoneNameHashes.Length;
            }

            VertexCount = (int)vertexStart;

            var shape = MergeBlendShapes(meshes);

            Shape = shape;

            SubMeshes = subMeshList.ToArray();
            Indices = indexList.ToArray();
            Skin = skinList.ToArray();
            BindPose = bindPoseList.ToArray();
            Vertices = vertexList.ToArray();
            Normals = normalList.ToArray();
            Colors = colorList.ToArray();
            UV1 = uv1List.ToArray();
            UV2 = uv2List.ToArray();
            Tangents = tangentList.ToArray();
            BoneNameHashes = boneNameHashList.ToArray();
            ParentMeshIndices = parentMeshIndices.ToArray();
        }

        public override string Name { get; }

        public override PrettySubMesh[] SubMeshes { get; }

        public override uint[] Indices { get; }

        public override BoneInfluence[][] Skin { get; }

        public override Matrix4x4[] BindPose { get; }

        public override int VertexCount { get; }

        public override Vector3[] Vertices { get; }

        public override Vector3[] Normals { get; }

        public override Vector4[] Colors { get; }

        public override Vector2[] UV1 { get; }

        public override Vector2?[] UV2 { get; }

        public override Vector3[] Tangents { get; }

        public override uint[] BoneNameHashes { get; }

        public override BlendShapeData Shape { get; }

        [NotNull, ItemNotNull]
        public string[] Names { get; }

        // Mapping: submesh index => name index
        [NotNull]
        public int[] ParentMeshIndices { get; }

        public int ComponentMeshCount { get; }

        [NotNull]
        public static CompositeMesh FromMeshes([NotNull, ItemNotNull] params PrettyMesh[] meshes) {
            return new CompositeMesh(meshes);
        }

        [NotNull]
        private static BlendShapeData MergeBlendShapes([NotNull, ItemNotNull] IReadOnlyList<PrettyMesh> meshes) {
            var vertices = new List<BlendShapeVertex>();
            var shapes = new List<MeshBlendShape>();
            var channels = new List<MeshBlendShapeChannel>();
            var weights = new List<float>();

            uint meshVertexIndexStart = 0;
            var totalFrameCount = 0;
            uint totalVertexCount = 0;

            foreach (var mesh in meshes) {
                var meshShape = mesh.Shape;

                if (meshShape != null) {
                    var channelFrameCount = 0;

                    foreach (var channel in meshShape.Channels) {
                        var chan = new MeshBlendShapeChannel(channel.Name, channel.NameHash, channel.FrameIndex + totalFrameCount, channel.FrameCount);

                        channelFrameCount += channel.FrameCount;

                        channels.Add(chan);
                    }

                    totalFrameCount += channelFrameCount;

                    weights.AddRange(meshShape.FullWeights);

                    uint shapeVertexCount = 0;

                    foreach (var s in meshShape.Shapes) {
                        var shape = new MeshBlendShape(s.FirstVertex + totalVertexCount, s.VertexCount, s.HasNormals, s.HasTangents);

                        shapeVertexCount += s.VertexCount;

                        shapes.Add(shape);
                    }

                    totalVertexCount += shapeVertexCount;

                    foreach (var v in meshShape.Vertices) {
                        var vertex = new BlendShapeVertex(v.Vertex, v.Normal, v.Tangent, v.Index + meshVertexIndexStart);

                        vertices.Add(vertex);
                    }
                }

                meshVertexIndexStart += (uint)mesh.VertexCount;
            }

            return new BlendShapeData(vertices.ToArray(), shapes.ToArray(), channels.ToArray(), weights.ToArray());
        }

    }
}
