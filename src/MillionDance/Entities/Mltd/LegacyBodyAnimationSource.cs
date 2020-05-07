using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    public sealed class LegacyBodyAnimationSource : IBodyAnimationSource {

        public LegacyBodyAnimationSource([NotNull] CharacterImasMotionAsset asset) {
            Asset = asset;
        }

        public BodyAnimation Convert() {
            return BodyAnimation.CreateFrom(Asset);
        }

        [NotNull]
        public CharacterImasMotionAsset Asset { get; }

    }
}
