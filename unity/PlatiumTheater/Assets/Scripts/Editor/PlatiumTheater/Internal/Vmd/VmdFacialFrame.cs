using JetBrains.Annotations;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdFacialFrame : VmdBaseFrame {

        internal VmdFacialFrame() {
        }

        [NotNull]
        public string FacialExpressionName { get; internal set; }

        public float Weight { get; internal set; }

    }
}
