using System.Diagnostics;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class SkeletonNode {

        internal SkeletonNode(int parentIndex, int axisIndex) {
            ParentIndex = parentIndex;
            AxisIndex = axisIndex;
        }

        public int ParentIndex {
            [DebuggerStepThrough]
            get;
        }

        public int AxisIndex {
            [DebuggerStepThrough]
            get;
        }

    }
}
