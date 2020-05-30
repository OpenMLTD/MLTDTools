using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class PrettySkeleton {

        internal PrettySkeleton([NotNull, ItemNotNull] SkeletonNode[] nodes, [NotNull] uint[] nodeIDs) {
            _nodes = nodes;
            _nodeIDs = nodeIDs;
        }

        internal PrettySkeleton([NotNull] Skeleton skeleton) {
            var nodeCount = skeleton.m_Node.Length;
            Debug.Assert(nodeCount == skeleton.m_ID.Length);

            var ids = new uint[nodeCount];

            for (var i = 0; i < nodeCount; i += 1) {
                ids[i] = skeleton.m_ID[i];
            }

            var nodes = new SkeletonNode[nodeCount];

            for (var i = 0; i < nodeCount; i += 1) {
                var n = skeleton.m_Node[i];
                nodes[i] = new SkeletonNode(n.m_ParentId, n.m_AxesId);
            }

            _nodes = nodes;
            _nodeIDs = ids;
        }

        [NotNull, ItemNotNull]
        public SkeletonNode[] Nodes {
            [DebuggerStepThrough]
            get => _nodes;
        }

        [NotNull]
        public uint[] NodeIDs {
            [DebuggerStepThrough]
            get => _nodeIDs;
        }

        [NotNull]
        private readonly SkeletonNode[] _nodes;

        [NotNull]
        private readonly uint[] _nodeIDs;

    }
}
