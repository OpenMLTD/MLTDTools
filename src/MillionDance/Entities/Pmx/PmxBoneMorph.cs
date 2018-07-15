using OpenTK;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxBoneMorph : PmxBaseMorph {

        internal PmxBoneMorph() {
        }

        public Vector3 Translation { get; internal set; }

        public Quaternion Rotation { get; internal set; }

    }
}
