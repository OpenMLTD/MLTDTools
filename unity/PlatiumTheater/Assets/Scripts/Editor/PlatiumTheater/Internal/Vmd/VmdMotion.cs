using JetBrains.Annotations;

namespace PlatiumTheater.Internal.Vmd {
    public sealed class VmdMotion {

        internal VmdMotion() {
        }

        [NotNull]
        public string ModelName { get; internal set; }

        public int Version { get; internal set; }

        [NotNull]
        public VmdBoneFrame[] BoneFrames { get; internal set; }

        [NotNull]
        public VmdFacialFrame[] FacialFrames { get; internal set; }

        [NotNull]
        public VmdCameraFrame[] CameraFrames { get; internal set; }

        [NotNull]
        public VmdLightFrame[] LightFrames { get; internal set; }

        [CanBeNull]
        public VmdIKFrame[] IKFrames { get; internal set; }

    }
}
