using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    // ReSharper disable once InconsistentNaming
    public sealed class IKLink {

        internal IKLink() {
        }

        [CanBeNull]
        public PmxBone Bone { get; internal set; }

        public bool IsLimited { get; internal set; }

        public Vector3 LowerBound { get; internal set; }

        public Vector3 UpperBound { get; internal set; }

        public const int InvalidBoneIndex = -1;

        internal int BoneIndex { get; set; } = InvalidBoneIndex;

    }
}
