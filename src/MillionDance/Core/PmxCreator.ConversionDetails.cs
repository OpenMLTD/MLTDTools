using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    partial class PmxCreator {

        public sealed class ConversionDetails {

            internal ConversionDetails([NotNull] string texturePrefix, bool applyToon, int toonNumber) {
                TexturePrefix = texturePrefix;
                ApplyToon = applyToon;
                ToonNumber = toonNumber;
            }

            [NotNull]
            public string TexturePrefix { get; }

            public bool ApplyToon { get; }

            public int ToonNumber { get; }

        }

    }
}
