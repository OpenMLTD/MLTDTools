using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Core.IO;

namespace OpenMLTD.MillionDance.Entities.Internal {
    internal sealed class CameraAnimationSet<T> : AnimationSet<T> {

        internal CameraAnimationSet(int cameraNumber, [CanBeNull] T @default, [CanBeNull] T another, [CanBeNull] T special, [CanBeNull] T gorgeous)
            : base(@default, another, special, gorgeous) {
            CameraNumber = cameraNumber;
        }

        /// <summary>
        /// Specified camera number (starting from 1). Can be <see cref="ResourceLoader.UnspecifiedCameraNumber"/> if not specified, or
        /// <see cref="ResourceLoader.InvalidCameraNumber"/> if specified camera is not found.
        /// </summary>
        /// <remarks>
        /// It can be checked against <see cref="MltdAnimation.MinCamera"/>.
        /// </remarks>
        public int CameraNumber { get; }

    }
}
