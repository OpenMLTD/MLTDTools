using OpenMLTD.MillionDance.Entities.Mltd.Sway;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxRigidBody : PmxBaseBody {

        internal PmxRigidBody() {
        }

        public int BoneIndex { get; internal set; }

        public BoundingBoxKind BoundingBoxKind { get; internal set; }

        public Vector3 BoundingBoxSize { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 RotationAngles { get; internal set; }

        public float Mass { get; internal set; }

        public float PositionDamping { get; internal set; }

        public float RotationDamping { get; internal set; }

        public float Restitution { get; internal set; }

        public float Friction { get; internal set; }

        public KineticMode KineticMode { get; internal set; }

        internal CoordSystemPart Part { get; set; }

    }
}
