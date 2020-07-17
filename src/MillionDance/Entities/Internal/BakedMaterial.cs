using System;
using System.Drawing;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Internal {
    // Should have the name "texture" not "material"?
    internal sealed class BakedMaterial : IDisposable {

        internal BakedMaterial([NotNull] string textureName, [NotNull] Bitmap bakedTexture) {
            TextureName = textureName;
            BakedTexture = bakedTexture;
        }

        [NotNull]
        public string TextureName { get; }

        [NotNull]
        public Bitmap BakedTexture { get; }

        public void Dispose() {
            BakedTexture.Dispose();
        }

    }
}
