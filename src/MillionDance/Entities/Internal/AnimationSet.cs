using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Internal {
    internal static class AnimationSet {

        [NotNull]
        public static AnimationSet<T> Create<T>([CanBeNull] T @default, [CanBeNull] T another, [CanBeNull] T special) {
            return new AnimationSet<T>(@default, another, special);
        }

    }
}
