using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal sealed partial class VmdCreator {

        public VmdCreator([NotNull] ConversionConfig conversionConfig, [NotNull] ScalingConfig scalingConfig) {
            _conversionConfig = conversionConfig;
            _scalingConfig = scalingConfig;
        }

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        public uint FixedFov { get; set; } = 20;

        [NotNull]
        private readonly ConversionConfig _conversionConfig;

        [NotNull]
        private readonly ScalingConfig _scalingConfig;

        private const string ModelName = "MODEL_00";

    }
}
