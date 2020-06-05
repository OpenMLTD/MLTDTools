using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdIkFrame : VmdBaseFrame {

        internal VmdIkFrame(int frameIndex, [NotNull, ItemNotNull] IkControl[] ikControls)
            : base(frameIndex) {
            IkControls = ikControls;
        }

        public bool Visible { get; internal set; }

        [NotNull, ItemNotNull]
        public IkControl[] IkControls { get; }

    }
}
