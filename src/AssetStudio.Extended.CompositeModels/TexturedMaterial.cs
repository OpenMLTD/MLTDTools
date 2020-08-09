using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class TexturedMaterial {

        internal TexturedMaterial([NotNull] string materialName, bool hasMainTexture, [CanBeNull] Texture2D mainTexture, bool hasSubTexture, [CanBeNull] Texture2D subTexture, [NotNull] TexturedMaterialExtraProperties extraProperties) {
            MaterialName = materialName;
            HasMainTexture = hasMainTexture;
            MainTexture = mainTexture;
            HasSubTexture = hasSubTexture;
            SubTexture = subTexture;
            ExtraProperties = extraProperties;
        }

        [NotNull]
        public string MaterialName { get; }

        /// <summary>
        /// Some materials does have a "_MainTex" field, but no texture associated.
        /// Under these situations, the Unity shader will use default value (set in the shader), usually "white" (RGBA={1,1,1,1}).
        /// </summary>
        public bool HasMainTexture { get; }

        [CanBeNull]
        public Texture2D MainTexture { get; }

        public bool HasSubTexture { get; }

        [CanBeNull]
        public Texture2D SubTexture { get; }

        [NotNull]
        public TexturedMaterialExtraProperties ExtraProperties { get; }

    }
}
