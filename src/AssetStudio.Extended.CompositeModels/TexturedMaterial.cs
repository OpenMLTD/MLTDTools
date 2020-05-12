using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class TexturedMaterial {

        internal TexturedMaterial([NotNull] string materialName, [NotNull] Texture2D mainTexture, [CanBeNull] Texture2D subTexture, bool flip) {
            MaterialName = materialName;
            MainTexture = mainTexture;
            SubTexture = subTexture;
            Flip = flip;
        }

        [NotNull]
        public string MaterialName { get; }

        [NotNull]
        public Texture2D MainTexture { get; }

        [CanBeNull]
        public Texture2D SubTexture { get; }

        public bool Flip { get; }

    }
}
