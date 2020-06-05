using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Mvd {
    public sealed class MvdMotion {

        internal MvdMotion([CanBeNull, ItemNotNull] MvdCameraMotion[] cameraMotions) {
            CameraMotions = cameraMotions ?? Array.Empty<MvdCameraMotion>();
        }

        public float Fps { get; internal set; }

        [NotNull, ItemNotNull]
        public MvdCameraMotion[] CameraMotions { get; }

    }
}
