using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdMotion {

        internal VmdMotion([NotNull] string modelName, [NotNull, ItemNotNull] IReadOnlyList<VmdBoneFrame> boneFrames,
            [NotNull, ItemNotNull] IReadOnlyList<VmdFacialFrame> facialFrames, [NotNull, ItemNotNull] IReadOnlyList<VmdCameraFrame> cameraFrames,
            [NotNull, ItemNotNull] IReadOnlyList<VmdLightFrame> lightFrames, [CanBeNull, ItemNotNull] IReadOnlyList<VmdIkFrame> ikFrames) {
            ModelName = modelName;
            BoneFrames = boneFrames;
            FacialFrames = facialFrames;
            CameraFrames = cameraFrames;
            LightFrames = lightFrames;
            IkFrames = ikFrames;
        }

        [NotNull]
        public string ModelName { get; }

        public int Version { get; } = 2;

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdBoneFrame> BoneFrames { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdFacialFrame> FacialFrames { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdCameraFrame> CameraFrames { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdLightFrame> LightFrames { get; }

        [CanBeNull, ItemNotNull]
        public IReadOnlyList<VmdIkFrame> IkFrames { get; }

    }
}
