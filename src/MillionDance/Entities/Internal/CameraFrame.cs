namespace MillionDance.Entities.Internal {
    public sealed class CameraFrame {

        internal CameraFrame() {
        }

        public float Time { get; internal set; }

        public float FocalLength { get; internal set; }

        /// <summary>
        /// Which cut this frame is in.
        /// </summary>
        public int Cut { get; internal set; }

        public float AngleX { get; internal set; }

        public float AngleY { get; internal set; }

        public float AngleZ { get; internal set; }

        public float PositionX { get; internal set; }

        public float PositionY { get; internal set; }

        public float PositionZ { get; internal set; }

        public float TargetX { get; internal set; }

        public float TargetY { get; internal set; }

        public float TargetZ { get; internal set; }

    }
}
