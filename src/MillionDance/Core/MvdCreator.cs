using System;
using System.Collections.Generic;
using AssetStudio;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mvd;
using OpenMLTD.MillionDance.Entities.Pmx;

namespace OpenMLTD.MillionDance.Core {
    public sealed partial class MvdCreator {

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        [NotNull]
        public MvdMotion CreateFrom([CanBeNull] CharacterImasMotionAsset bodyMotion, [CanBeNull] Avatar avatar, [CanBeNull] PmxModel mltdPmxModel,
            [CanBeNull] CharacterImasMotionAsset cameraMotion,
            [CanBeNull] ScenarioObject lipSync, [CanBeNull] ScenarioObject facialExpressions,
            int songPosition) {
            IReadOnlyList<MvdCameraMotion> cameraFrames;

            if (ProcessCameraFrames && cameraMotion != null) {
                cameraFrames = CreateCameraMotions(cameraMotion);
            } else {
                cameraFrames = Array.Empty<MvdCameraMotion>();
            }

            var mvd = new MvdMotion(cameraFrames);

            if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                mvd.Fps = FrameRate.Mmd;
            } else {
                mvd.Fps = FrameRate.Mltd;
            }

            return mvd;
        }

    }
}
