using JetBrains.Annotations;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdIKFrame : VmdBaseFrame {

        internal VmdIKFrame() {
        }

        public bool Visible { get; internal set; }

        [NotNull]
        public IKControl[] IKControls { get; internal set; }

    }
}
