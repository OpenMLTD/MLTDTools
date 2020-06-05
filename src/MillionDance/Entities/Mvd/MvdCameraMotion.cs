using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Mvd {
    public sealed class MvdCameraMotion : MvdBaseMotion {

        internal MvdCameraMotion([NotNull] MvdCameraObject camera, [NotNull, ItemNotNull] MvdCameraFrame[] cameraFrames, [NotNull, ItemNotNull] MvdCameraPropertyFrame[] cameraPropertyFrames) {
            Camera = camera;
            CameraFrames = cameraFrames;
            CameraPropertyFrames = cameraPropertyFrames;
        }

        [NotNull]
        public string DisplayName { get; internal set; } = string.Empty;

        [NotNull]
        public string EnglishName { get; internal set; } = string.Empty;

        [NotNull]
        public MvdCameraObject Camera { get; }

        [NotNull, ItemNotNull]
        public MvdCameraFrame[] CameraFrames { get; }

        [NotNull, ItemNotNull]
        public MvdCameraPropertyFrame[] CameraPropertyFrames { get; }

    }
}
