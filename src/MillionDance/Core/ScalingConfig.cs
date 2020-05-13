using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal sealed class ScalingConfig {

        public ScalingConfig([NotNull] ConversionConfig conversionConfig) {
            _conversionConfig = conversionConfig;
        }

        // Character height, in meters.
        // Setting this value to character's real height can affect how tall the exported model is. Useful for height comparison. XD
        public float CharacterHeight { get; set; } = 1.6f;

        public float ScalePmxToUnity {
            get {
                if (_conversionConfig.ApplyPmxCharacterHeight) {
                    return DefaultScaleMmdToUnity / (CharacterHeight / StandardCharacterHeight);
                } else {
                    return DefaultScaleMmdToUnity;
                }
            }
        }

        public float ScaleUnityToPmx {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScalePmxToUnity;
        }

        public float ScaleVmdToUnity { get; } = DefaultScaleMmdToUnity;

        public float ScaleUnityToVmd {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScaleVmdToUnity;
        }

        internal const float StandardCharacterHeight = 1.6f;

        private const float DefaultScaleMmdToUnity = 0.08f;

        [NotNull]
        private readonly ConversionConfig _conversionConfig;

    }
}
