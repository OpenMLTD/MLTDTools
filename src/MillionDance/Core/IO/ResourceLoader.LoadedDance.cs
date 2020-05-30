using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Core.IO {
    partial class ResourceLoader {

        internal class LoadedDance {

            public LoadedDance([NotNull] AnimationSet<IBodyAnimationSource> animationSet, int suggestedPosition) {
                AnimationSet = animationSet;
                SuggestedPosition = suggestedPosition;
            }

            [NotNull]
            public AnimationSet<IBodyAnimationSource> AnimationSet { get; }

            public int SuggestedPosition { get; }

        }

    }
}
