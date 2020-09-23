using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Core.IO;

namespace OpenMLTD.MillionDance.Entities.Internal {
    internal sealed class DanceAnimationSet<T> : AnimationSet<T> {

        internal DanceAnimationSet(int suggestedPosition, [CanBeNull] T @default, [CanBeNull] T another, [CanBeNull] T special, [CanBeNull] T gorgeous)
            : base(@default, another, special, gorgeous) {
            SuggestedPosition = suggestedPosition;
        }

        /// <summary>
        /// Suggested dance position. Can be <see cref="ResourceLoader.InvalidDancePosition"/> if not set.
        /// </summary>
        /// <remarks>
        /// It can be checked against <see cref="MltdAnimation.MinDance"/> and <see cref="MltdAnimation.MaxDance"/>.
        /// </remarks>
        public int SuggestedPosition { get; }

    }
}
