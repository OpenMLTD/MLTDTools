namespace OpenMLTD.MillionDance.Entities.Mvd {
    public abstract class MvdBaseFrame {

        protected MvdBaseFrame(long frameNumber) {
            FrameNumber = frameNumber;
        }

        public long FrameNumber { get; }

    }
}
