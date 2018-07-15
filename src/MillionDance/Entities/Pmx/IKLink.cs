using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Entities.Pmx {
    // ReSharper disable once InconsistentNaming
    public sealed class IKLink {

        internal IKLink() {
        }

        public int BoneIndex { get; internal set; } = InvalidBoneIndex;

        [CanBeNull]
        public PmxBone Bone { get; internal set; }

        public bool IsLimited { get; internal set; }

        public Vector3 LowerBound { get; internal set; }

        public Vector3 UpperBound { get; internal set; }

        public const int InvalidBoneIndex = -1;

    }
}
