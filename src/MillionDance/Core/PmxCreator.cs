using JetBrains.Annotations;
using OpenMLTD.MillionDance.Utilities;

namespace OpenMLTD.MillionDance.Core {
    internal sealed partial class PmxCreator {

        public PmxCreator([NotNull] ConversionConfig conversionConfig, [NotNull] ScalingConfig scalingConfig, [NotNull] BoneLookup boneLookup) {
            _conversionConfig = conversionConfig;
            _scalingConfig = scalingConfig;
            _boneLookup = boneLookup;
        }

        [NotNull]
        private readonly ConversionConfig _conversionConfig;

        [NotNull]
        private readonly ScalingConfig _scalingConfig;

        [NotNull]
        private readonly BoneLookup _boneLookup;

    }
}
