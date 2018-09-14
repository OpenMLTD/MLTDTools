using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Mltd.Sway {
    public sealed class SwayManager : SwayBase {

        internal SwayManager() {
        }

        public Vector3 Gravity { get; internal set; }

        public float StiffnessForce { get; internal set; }

        public float DragForce { get; internal set; }

        public float FollowForce { get; internal set; }

        public float LineMoveLimit { get; internal set; }

        public float SideLineMoveLimit { get; internal set; }

        public override string ToString() {
            return $"SwayManager \"{Path}\"";
        }

    }
}
