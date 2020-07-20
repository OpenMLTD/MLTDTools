using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal sealed class ScalingConfig {

        public ScalingConfig([NotNull] ConversionConfig conversionConfig) {
            _conversionConfig = conversionConfig;
        }

        // Character height, in meters.
        // Setting this value to character's real height can affect how tall the exported model is. Useful for height comparison. XD
        public float CharacterHeight { get; set; } = StandardCharacterHeight;

        public float CharacterHeightScalingFactor {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CharacterHeight / StandardCharacterHeight;
        }

        public float ScalePmxToUnity {
            get {
                if (_conversionConfig.ApplyPmxCharacterHeight) {
                    return DefaultScaleMmdToUnity / CharacterHeightScalingFactor;
                } else {
                    return DefaultScaleMmdToUnity;
                }
            }
        }

        public float ScaleUnityToPmx {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScalePmxToUnity;
        }

        // This should remain constant. The only values that need to scale are the positional movements.
        // They are idol's position projected on stage, which have nothing to do with idol heights, i.e. you have
        // to run the same distance no matter how tall you are.
        public float ScaleVmdToUnity { get; } = DefaultScaleMmdToUnity;

        public float ScaleUnityToVmd {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScaleVmdToUnity;
        }

        internal const uint StandardCharacterHeightInCentimeters = 160;

        private const float StandardCharacterHeight = StandardCharacterHeightInCentimeters / 100.0f;

        private const float DefaultScaleMmdToUnity = 0.08f;

        [NotNull]
        private readonly ConversionConfig _conversionConfig;

    }
}
