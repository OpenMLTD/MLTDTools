using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Entities.Mvd {
    public sealed class MvdMotion {

        internal MvdMotion([NotNull, ItemNotNull] IReadOnlyList<MvdCameraMotion> cameraMotions) {
            CameraMotions = cameraMotions;
        }

        public float Fps { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<MvdCameraMotion> CameraMotions { get; }

    }
}
