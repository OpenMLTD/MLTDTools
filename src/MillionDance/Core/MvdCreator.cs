using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal sealed partial class MvdCreator {

        public MvdCreator([NotNull] ConversionConfig conversionConfig, [NotNull] ScalingConfig scalingConfig) {
            _conversionConfig = conversionConfig;
            _scalingConfig = scalingConfig;
        }

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        [NotNull]
        private readonly ConversionConfig _conversionConfig;

        [NotNull]
        private readonly ScalingConfig _scalingConfig;

    }
}
