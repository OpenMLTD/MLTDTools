using OpenTK;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxJoint : IPmxNamedObject {

        internal PmxJoint() {
        }

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        public JointKind Kind { get; internal set; }

        public int BodyIndex1 { get; internal set; }

        public int BodyIndex2 { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 RotationAngles { get; internal set; }

        public Vector3 LimitMoveLower { get; internal set; }

        public Vector3 LimitMoveUpper { get; internal set; }

        public Vector3 LimitAngleLower { get; internal set; }

        public Vector3 LimitAngleUpper { get; internal set; }

        public Vector3 SpConst_Move { get; internal set; }

        public Vector3 SpConst_Rotate { get; internal set; }

    }
}
