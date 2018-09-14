using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdFacialFrame : VmdBaseFrame {

        internal VmdFacialFrame(int frameIndex, [NotNull] string facialExpressionName)
            : base(frameIndex) {
            FacialExpressionName = facialExpressionName;
        }

        [NotNull]
        public string FacialExpressionName { get; }

        public float Weight { get; internal set; }

    }
}
