namespace MillionDance.Core {
    internal static class ScalingConfig {

        // Character height, in meters.
        // Setting this value to character's real height can affect how tall the exported model is. Useful for height comparison. XD
        public const float CharacterHeight = 1.58f;

        public const float ScalePmxToUnity = 0.08f / (CharacterHeight / StandardCharacterHeight);
        public const float ScaleUnityToPmx = 1 / ScalePmxToUnity;

        public const float ScaleVmdToUnity = 0.08f;
        public const float ScaleUnityToVmd = 1 / ScaleVmdToUnity;

        private const float StandardCharacterHeight = 1.6f;

    }
}
