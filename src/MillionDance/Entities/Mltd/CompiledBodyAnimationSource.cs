using AssetStudio;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    public sealed class CompiledBodyAnimationSource : IBodyAnimationSource {

        public CompiledBodyAnimationSource([NotNull] AnimationClip animationClip) {
            AnimationClip = animationClip;
        }

        public BodyAnimation Convert() {
            return BodyAnimation.CreateFrom(AnimationClip);
        }

        [NotNull]
        public AnimationClip AnimationClip { get; }

    }
}
