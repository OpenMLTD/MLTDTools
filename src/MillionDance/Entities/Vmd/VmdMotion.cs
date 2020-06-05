using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdMotion {

        internal VmdMotion([NotNull] string modelName, [CanBeNull, ItemNotNull] VmdBoneFrame[] boneFrames,
            [CanBeNull, ItemNotNull] VmdFacialFrame[] facialFrames, [CanBeNull, ItemNotNull] VmdCameraFrame[] cameraFrames,
            [CanBeNull, ItemNotNull] VmdLightFrame[] lightFrames, [CanBeNull, ItemNotNull] VmdIkFrame[] ikFrames) {
            ModelName = modelName;
            BoneFrames = boneFrames ?? Array.Empty<VmdBoneFrame>();
            FacialFrames = facialFrames ?? Array.Empty<VmdFacialFrame>();
            CameraFrames = cameraFrames ?? Array.Empty<VmdCameraFrame>();
            LightFrames = lightFrames ?? Array.Empty<VmdLightFrame>();
            IkFrames = ikFrames;
        }

        [NotNull]
        public string ModelName { get; }

        public int Version { get; } = 2;

        [NotNull, ItemNotNull]
        public VmdBoneFrame[] BoneFrames { get; }

        [NotNull, ItemNotNull]
        public VmdFacialFrame[] FacialFrames { get; }

        [NotNull, ItemNotNull]
        public VmdCameraFrame[] CameraFrames { get; }

        [NotNull, ItemNotNull]
        public VmdLightFrame[] LightFrames { get; }

        [CanBeNull, ItemNotNull]
        public VmdIkFrame[] IkFrames { get; }

    }
}
