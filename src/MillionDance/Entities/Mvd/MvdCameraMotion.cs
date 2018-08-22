using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Entities.Mvd {
    public sealed class MvdCameraMotion : MvdBaseMotion {

        internal MvdCameraMotion([NotNull] MvdCameraObject camera, [NotNull, ItemNotNull] IReadOnlyList<MvdCameraFrame> cameraFrames, [NotNull, ItemNotNull] IReadOnlyList<MvdCameraPropertyFrame> cameraPropertyFrames) {
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
        public IReadOnlyList<MvdCameraFrame> CameraFrames { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<MvdCameraPropertyFrame> CameraPropertyFrames { get; }

    }
}
