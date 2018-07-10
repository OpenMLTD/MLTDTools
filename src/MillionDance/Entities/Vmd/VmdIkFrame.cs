using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Entities.Vmd {
    public sealed class VmdIkFrame : VmdBaseFrame {

        internal VmdIkFrame(int frameIndex, [NotNull, ItemNotNull] IReadOnlyList<IkControl> ikControls)
            : base(frameIndex) {
            IkControls = ikControls;
        }

        public bool Visible { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IkControl> IkControls { get; }

    }
}
