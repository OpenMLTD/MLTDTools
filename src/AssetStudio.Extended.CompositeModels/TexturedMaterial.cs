using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class TexturedMaterial {

        internal TexturedMaterial([NotNull] string materialName, [NotNull] Texture2D mainTexture, [CanBeNull] Texture2D subTexture, bool shouldFlip, bool shouldApplyToon) {
            MaterialName = materialName;
            MainTexture = mainTexture;
            SubTexture = subTexture;
            ShouldFlip = shouldFlip;
            ShouldApplyToon = shouldApplyToon;
        }

        [NotNull]
        public string MaterialName { get; }

        [NotNull]
        public Texture2D MainTexture { get; }

        [CanBeNull]
        public Texture2D SubTexture { get; }

        public bool ShouldFlip { get; }

        public bool ShouldApplyToon { get; }

    }
}
