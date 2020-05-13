using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class VmdMotion {

        internal VmdMotion([NotNull] string modelName, [CanBeNull, ItemNotNull] IReadOnlyList<VmdBoneFrame> boneFrames,
            [CanBeNull, ItemNotNull] IReadOnlyList<VmdFacialFrame> facialFrames, [CanBeNull, ItemNotNull] IReadOnlyList<VmdCameraFrame> cameraFrames,
            [CanBeNull, ItemNotNull] IReadOnlyList<VmdLightFrame> lightFrames, [CanBeNull, ItemNotNull] IReadOnlyList<VmdIkFrame> ikFrames) {
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
