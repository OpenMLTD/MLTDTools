namespace MillionDance.Entities.Mvd {
    public sealed class MvdCameraPropertyFrame : MvdBaseFrame {

        internal MvdCameraPropertyFrame(long frameNumber)
            : base(frameNumber) {
        }

        public bool Enabled { get; internal set; }

        public bool IsPerspective { get; internal set; }

        public float Alpha { get; internal set; }

        public bool EffectEnabled { get; internal set; }

        public bool DynamicFovEnabled { get; internal set; }

        public float DynamicFovRate { get; internal set; }

        public float DynamicFovCoefficient { get; internal set; }

        public int RelatedModelId { get; internal set; }

        public int RelatedBoneId { get; internal set; }

    }
}
