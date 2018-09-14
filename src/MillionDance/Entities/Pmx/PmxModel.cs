using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Utilities;

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
        public IReadOnlyList<PmxVertex> Vertices { get; internal set; } = EmptyArray.Of<PmxVertex>();

        /// <summary>
        /// Triangles, in which every int tuple (v1, v2, v3) represents a triangle.
        /// </summary>
        [NotNull]
        public IReadOnlyList<int> FaceTriangles { get; internal set; } = EmptyArray.Of<int>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxMaterial> Materials { get; internal set; } = EmptyArray.Of<PmxMaterial>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBone> Bones { get; internal set; } = EmptyArray.Of<PmxBone>();

        [NotNull]
        public IReadOnlyDictionary<string, PmxBone> BonesDictionary { get; internal set; } = new Dictionary<string, PmxBone>();

        [NotNull]
        public IReadOnlyList<int> RootBoneIndices { get; internal set; } = EmptyArray.Of<int>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxMorph> Morphs { get; internal set; } = EmptyArray.Of<PmxMorph>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxNode> Nodes { get; internal set; } = EmptyArray.Of<PmxNode>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxRigidBody> RigidBodies { get; internal set; } = EmptyArray.Of<PmxRigidBody>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxJoint> Joints { get; internal set; } = EmptyArray.Of<PmxJoint>();

        [CanBeNull, ItemNotNull]
        public IReadOnlyList<PmxSoftBody> SoftBodies { get; internal set; }

    }
}
