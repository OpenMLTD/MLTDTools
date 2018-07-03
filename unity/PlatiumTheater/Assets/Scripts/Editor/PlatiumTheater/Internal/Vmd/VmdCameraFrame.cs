using UnityEngine;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdCameraFrame : VmdBaseFrame {

        internal VmdCameraFrame() {
            Interpolation = new byte[6, 4];
            Unknown = new byte[3];
        }

        public float Distance { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 Orientation { get; internal set; }

        public byte[,] Interpolation { get; private set; }

        public float FieldOfView { get; internal set; }

        public byte[] Unknown { get; private set; }

    }
}
