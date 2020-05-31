using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    partial class PmxCreator {

        public sealed class ConversionDetails {

            internal ConversionDetails([NotNull] string texturePrefix, bool applyToon, int skinToonNumber, int clothesToonNumber) {
                TexturePrefix = texturePrefix;
                ApplyToon = applyToon;
                SkinToonNumber = skinToonNumber;
                ClothesToonNumber = clothesToonNumber;
            }

            [NotNull]
            public string TexturePrefix { get; }

            public bool ApplyToon { get; }

            public int SkinToonNumber { get; }

            public int ClothesToonNumber { get; }

        }

    }
}
