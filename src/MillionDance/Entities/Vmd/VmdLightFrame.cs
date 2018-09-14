using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdLightFrame : VmdBaseFrame {

        internal VmdLightFrame(int frameIndex)
            : base(frameIndex) {
        }

        public Vector3 Color { get; internal set; }

        public Vector3 Position { get; internal set; }

    }
}
