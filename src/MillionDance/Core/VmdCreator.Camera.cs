using System.Collections.Generic;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateCameraMotion([CanBeNull] CharacterImasMotionAsset mainCamera, [NotNull] ScenarioObject baseScenario, [CanBeNull] CharacterImasMotionAsset cameraAppeal, AppealType appealType) {
            VmdCameraFrame[] frames;

            if (ProcessCameraFrames && mainCamera != null) {
                frames = CreateCameraFrames(mainCamera, baseScenario, cameraAppeal, FixedFov, appealType);
            } else {
                frames = null;
            }

            return new VmdMotion(ModelName, null, null, frames, null, null);
        }

        [NotNull, ItemNotNull]
        private VmdCameraFrame[] CreateCameraFrames([CanBeNull] CharacterImasMotionAsset mainCamera, [NotNull] ScenarioObject baseScenario, [CanBeNull] CharacterImasMotionAsset cameraAppeal, uint fixedFov, AppealType appealType) {
            // Here we reuse the logic in MVD camera frame computation
            var mvdCreator = new MvdCreator(_conversionConfig, _scalingConfig) {
                ProcessCameraFrames = true,
                ProcessBoneFrames = false,
                ProcessFacialFrames = false,
                ProcessLightFrames = false,
            };

            var mvdMotion = mvdCreator.CreateCameraMotion(mainCamera, baseScenario, cameraAppeal, appealType);
            var mvdFrames = mvdMotion.CameraMotions[0].CameraFrames;

            var cameraFrameList = new List<VmdCameraFrame>();

            foreach (var mvdFrame in mvdFrames) {
                var vmdFrame = new VmdCameraFrame((int)mvdFrame.FrameNumber);

                vmdFrame.Length = mvdFrame.Distance;
                vmdFrame.Position = mvdFrame.Position;
                vmdFrame.Orientation = mvdFrame.Rotation;

                // VMD does not have good support for animated FOV. So here just use a constant to avoid "jittering".
                // The drawback is, some effects (like the first zooming cut in Shooting Stars) will not be able to achieve.
                //var fov = FocalLengthToFov(frame.FocalLength);
                //vmdFrame.FieldOfView = (uint)fov;

                vmdFrame.FieldOfView = fixedFov;

                cameraFrameList.Add(vmdFrame);
            }

            return cameraFrameList.ToArray();
        }

    }
}
