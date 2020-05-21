using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Internal {
    /// <summary>
    /// A set of animations
    /// </summary>
    /// <typeparam name="T">The type containing animation data</typeparam>
    internal sealed class AnimationSet<T> {

        public AnimationSet([CanBeNull] T @default, [CanBeNull] T another, [CanBeNull] T special) {
            Default = @default;
            Special = special;
            Another = another;
        }

        /// <summary>
        /// Default animations
        /// </summary>
        [CanBeNull]
        public T Default {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        /// Animations for another appeal (apa)
        /// </summary>
        [CanBeNull]
        public T Another {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        /// Animations for special appeal (apg)
        /// </summary>
        [CanBeNull]
        public T Special {
            [DebuggerStepThrough]
            get;
        }

    }
}
