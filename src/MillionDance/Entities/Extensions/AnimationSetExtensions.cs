using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Entities.Extensions {
    internal static class AnimationSetExtensions {

        public static void Deconstruct<T>([NotNull] this AnimationSet<T> animationSet, [CanBeNull] out T @default, [CanBeNull] out T another, [CanBeNull] out T special) {
            @default = animationSet.Default;
            another = animationSet.Another;
            special = animationSet.Special;
        }

    }
}
