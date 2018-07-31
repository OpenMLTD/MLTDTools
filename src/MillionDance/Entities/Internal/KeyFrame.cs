using System.Diagnostics;
using JetBrains.Annotations;

namespace MillionDance.Entities.Internal {
    public sealed class KeyFrame {

        public KeyFrame(int frameIndex, [NotNull] string path) {
            FrameIndex = frameIndex;
            Time = 1.0f / 60.0f * frameIndex;
            Path = path;
        }

        public int FrameIndex { get; }

        public float Time { get; }

        [NotNull]
        public string Path { get; }

        public bool HasPositions {
            [DebuggerStepThrough]
            get => PositionX != null && PositionY != null && PositionZ != null;
        }

        [CanBeNull]
        public float? PositionX { get; set; }

        [CanBeNull]
        public float? PositionY { get; set; }

        [CanBeNull]
        public float? PositionZ { get; set; }

        public bool HasRotations {
            [DebuggerStepThrough]
            get => AngleX != null && AngleY != null && AngleZ != null;
        }

        // Degree
        [CanBeNull]
        public float? AngleX { get; set; }

        // Degree
        [CanBeNull]
        public float? AngleY { get; set; }

        // Degree
        [CanBeNull]
        public float? AngleZ { get; set; }

        public override string ToString() {
            return $"KeyFrame #{FrameIndex} ({Time}) Path={Path}";
        }

    }
}
