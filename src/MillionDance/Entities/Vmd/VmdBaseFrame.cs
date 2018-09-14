namespace OpenMLTD.MillionDance.Entities.Vmd {
    public abstract class VmdBaseFrame {

        protected VmdBaseFrame(int frameIndex) {
            FrameIndex = frameIndex;
        }

        public int FrameIndex { get; }

    }
}
