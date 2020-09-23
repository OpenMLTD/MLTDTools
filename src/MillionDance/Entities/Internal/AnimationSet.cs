using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Internal {
    internal static class AnimationSet {

        [NotNull]
        public static CameraAnimationSet<T> CreateCamera<T>(int cameraNumber, [CanBeNull] T @default, [CanBeNull] T special, [CanBeNull] T another, [CanBeNull] T gorgeous) {
            return new CameraAnimationSet<T>(cameraNumber, @default, another, special, gorgeous);
        }

        [NotNull]
        public static DanceAnimationSet<T> CreateDance<T>(int suggestedPosition, [CanBeNull] T @default, [CanBeNull] T special, [CanBeNull] T another, [CanBeNull] T gorgeous) {
            return new DanceAnimationSet<T>(suggestedPosition, @default, another, special, gorgeous);
        }

    }
}
