using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    // TODO: This class should be placed in MillionDance or somewhere else, definitely not in AssetStudio.Extended.*.
    //       But I can't find a good replacement for it...
    public sealed class TexturedMaterialExtraProperties {

        static TexturedMaterialExtraProperties() {
            Head = new TexturedMaterialExtraProperties(false);
            Body = new TexturedMaterialExtraProperties(true);
        }

        private TexturedMaterialExtraProperties(bool shouldFlip) {
            ShouldFlip = shouldFlip;
        }

        public bool ShouldFlip { get; }

        [NotNull]
        public static TexturedMaterialExtraProperties Head { get; }

        [NotNull]
        public static TexturedMaterialExtraProperties Body { get; }

    }
}
