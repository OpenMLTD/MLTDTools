using System.Collections.Generic;
using AssetStudio;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mvd;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Utilities;

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
                cameraFrames = EmptyArray.Of<MvdCameraMotion>();
            }

            var mvd = new MvdMotion(cameraFrames);

            if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                mvd.Fps = 30;
            } else {
                mvd.Fps = 60;
            }

            return mvd;
        }

    }
}
