using System.Runtime.CompilerServices;

namespace OpenMLTD.MillionDance.Core {
    internal static class ScalingConfig {

        // Character height, in meters.
        // Setting this value to character's real height can affect how tall the exported model is. Useful for height comparison. XD
        public static float CharacterHeight { get; set; } = 1.6f;

        public static float ScalePmxToUnity {
            get {
                if (ConversionConfig.Current.ApplyPmxCharacterHeight) {
                    return DefaultScaleMmdToUnity / (CharacterHeight / StandardCharacterHeight);
                } else {
                    return DefaultScaleMmdToUnity;
                }
            }
        }

        public static float ScaleUnityToPmx {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScalePmxToUnity;
        }

        public static float ScaleVmdToUnity { get; } = DefaultScaleMmdToUnity;

        public static float ScaleUnityToVmd {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1 / ScaleVmdToUnity;
        }

        internal const float StandardCharacterHeight = 1.6f;
        private const float DefaultScaleMmdToUnity = 0.08f;

    }
}
