using OpenTK;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxImpulseMorph : PmxBaseMorph {

        internal PmxImpulseMorph() {
        }

        public bool IsLocal { get; internal set; }

        public Vector3 Torque { get; internal set; }

        public Vector3 Velocity { get; internal set; }

    }
}
