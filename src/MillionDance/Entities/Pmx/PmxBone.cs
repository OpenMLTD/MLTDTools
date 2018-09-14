using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxBone : IPmxNamedObject {

        internal PmxBone() {
        }

        public const int InvalidBoneIndex = -1;

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string Name { get; internal set; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string NameEnglish { get; internal set; }

        public Vector3 InitialPosition {
            get => _initialPosition;
            internal set {
                _initialPosition = value;
                BindingPoseMatrixInverse = Matrix4.CreateTranslation(-value);
            }
        }

        public int BoneIndex { get; internal set; }

        public Vector3 CurrentPosition { get; internal set; }

        public Vector3 Axis { get; internal set; }

        public Quaternion InitialRotation { get; internal set; } = Quaternion.Identity;

        public Quaternion CurrentRotation { get; internal set; } = Quaternion.Identity;

        public int Level { get; internal set; }

        public int To_Bone { get; internal set; } = InvalidBoneIndex;

        public Vector3 To_Offset { get; internal set; }

        [CanBeNull]
        public PmxBone AppendParent { get; internal set; }

        [CanBeNull]
        public PmxBone ExternalParent { get; internal set; }

        public float AppendRatio { get; internal set; } = 1;

        public BoneFlags Flags { get; internal set; } = BoneFlags.Enabled | BoneFlags.Visible;

        [CanBeNull]
        public PmxIK IK { get; internal set; }

        [CanBeNull]
        public PmxBone Parent { get; internal set; }

        // TODO: Setter should be internal.
        public Matrix4 WorldMatrix { get; set; }

        // TODO: Whole property should be internal.
        public Matrix4 LocalMatrix { get; set; }

        public Matrix4 SkinMatrix { get; internal set; }

        public bool IsIKSolved { get; internal set; }

        public override string ToString() {
            var description = GetSimpleDescription();

            if (Parent != null) {
                var parentInfo = $" Parent \"{Parent.Name}\"";

                description += parentInfo;
            }

            return description;
        }

        internal int ParentIndex { get; set; } = InvalidBoneIndex;

        internal int AppendParentIndex { get; set; } = InvalidBoneIndex;

        internal int ExternalParentIndex { get; set; } = InvalidBoneIndex;

        internal bool IsTransformCalculated { get; set; }

        internal Matrix4 BindingPoseMatrixInverse { get; private set; }

        internal Vector3 AnimatedTranslation { get; set; }

        internal Quaternion AnimatedRotation { get; set; } = Quaternion.Identity;

        internal bool IsMltdKeyBone { get; set; }

        private string GetSimpleDescription() {
            return $"Bone \"{Name}\" [{BoneIndex}] (Position: {CurrentPosition}; Rotation: {CurrentRotation})";
        }

        private Vector3 _initialPosition;

    }
}
