using UnityEngine;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdLightFrame : VmdBaseFrame {

        internal VmdLightFrame() {
        }

        public Vector3 Color { get; internal set; }

        public Vector3 Position { get; internal set; }

    }
}
