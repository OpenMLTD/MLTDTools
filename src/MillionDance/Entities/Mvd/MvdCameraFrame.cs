using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Mvd {
    public sealed class MvdCameraFrame : MvdBaseFrame {

        internal MvdCameraFrame(long frameNumber)
            : base(frameNumber) {
        }

        public float Distance { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 Rotation { get; internal set; }

        // Field of view in radians
        public float FieldOfView { get; internal set; }

        public bool IsSpline { get; internal set; }

        [NotNull]
        public InterpolationPair TranslationInterpolation { get; internal set; } = InterpolationPair.Linear();

        [NotNull]
        public InterpolationPair RotationInterpolation { get; internal set; } = InterpolationPair.Linear();

        [NotNull]
        public InterpolationPair DistanceInterpolation { get; internal set; } = InterpolationPair.Linear();

        [NotNull]
        public InterpolationPair FovInterpolation { get; internal set; } = InterpolationPair.Linear();

    }
}
