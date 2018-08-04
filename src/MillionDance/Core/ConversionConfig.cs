namespace MillionDance.Core {
    internal static class ConversionConfig {

        // Character height, in meters.
        // Setting this value to character's real height can affect how tall the exported model is. Useful for height comparison. XD
        public const float CharacterHeight = 1.58f;

        public const float ScaleMmdToUnity = 0.08f * (StandardCharacterHeight / CharacterHeight);
        public const float ScaleUnityToMmd = 1 / ScaleMmdToUnity / (StandardCharacterHeight / CharacterHeight);

        private const float StandardCharacterHeight = 1.6f;

    }
}
