using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class TexturedMaterial {

        internal TexturedMaterial([NotNull] string materialName, [NotNull] Texture2D mainTexture, [CanBeNull] Texture2D subTexture, [NotNull] TexturedMaterialExtraProperties extraProperties) {
            MaterialName = materialName;
            MainTexture = mainTexture;
            SubTexture = subTexture;
            ExtraProperties = extraProperties;
        }

        [NotNull]
        public string MaterialName { get; }

        [NotNull]
        public Texture2D MainTexture { get; }

        [CanBeNull]
        public Texture2D SubTexture { get; }

        [NotNull]
        public TexturedMaterialExtraProperties ExtraProperties { get; }

    }
}
