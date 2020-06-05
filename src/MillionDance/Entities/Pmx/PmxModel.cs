using System;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxModel : IPmxNamedObject {

        internal PmxModel() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        [NotNull]
        public string Comment { get; internal set; } = string.Empty;

        [NotNull]
        public string CommentEnglish { get; internal set; } = string.Empty;

        public int RootNodeIndex { get; internal set; }

        public int FacialExpressionNodeIndex { get; internal set; }

        [NotNull, ItemNotNull]
        public PmxVertex[] Vertices { get; internal set; } = Array.Empty<PmxVertex>();

        /// <summary>
        /// Triangles, in which every int tuple (v1, v2, v3) represents a triangle.
        /// </summary>
        [NotNull]
        public int[] FaceTriangles { get; internal set; } = Array.Empty<int>();

        [NotNull, ItemNotNull]
        public PmxMaterial[] Materials { get; internal set; } = Array.Empty<PmxMaterial>();

        [NotNull, ItemNotNull]
        public PmxBone[] Bones { get; internal set; } = Array.Empty<PmxBone>();

        [NotNull]
        public int[] RootBoneIndices { get; internal set; } = Array.Empty<int>();

        [NotNull, ItemNotNull]
        public PmxMorph[] Morphs { get; internal set; } = Array.Empty<PmxMorph>();

        [NotNull, ItemNotNull]
        public PmxNode[] Nodes { get; internal set; } = Array.Empty<PmxNode>();

        [NotNull, ItemNotNull]
        public PmxRigidBody[] RigidBodies { get; internal set; } = Array.Empty<PmxRigidBody>();

        [NotNull, ItemNotNull]
        public PmxJoint[] Joints { get; internal set; } = Array.Empty<PmxJoint>();

        [CanBeNull, ItemNotNull]
        public PmxSoftBody[] SoftBodies { get; internal set; }

    }
}
