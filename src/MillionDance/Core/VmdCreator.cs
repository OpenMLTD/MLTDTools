using System;
using System.Collections.Generic;
using System.Linq;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    public sealed partial class VmdCreator {

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        public uint FixedFov { get; set; } = 20;

        [NotNull]
        public VmdMotion CreateFrom([CanBeNull] IBodyAnimationSource bodyAnimationSource, [CanBeNull] PrettyAvatar avatar, [CanBeNull] PmxModel mltdPmxModel,
            [CanBeNull] CharacterImasMotionAsset cameraMotion,
            [CanBeNull] ScenarioObject baseScenario, [CanBeNull] ScenarioObject facialExpr,
            int songPosition) {
            IReadOnlyList<VmdBoneFrame> boneFrames;
            IReadOnlyList<VmdCameraFrame> cameraFrames;
            IReadOnlyList<VmdFacialFrame> facialFrames;
            IReadOnlyList<VmdLightFrame> lightFrames;

            if (ProcessBoneFrames && (bodyAnimationSource != null && avatar != null && mltdPmxModel != null)) {
                boneFrames = CreateBoneFrames(bodyAnimationSource, avatar, mltdPmxModel);
            } else {
                boneFrames = Array.Empty<VmdBoneFrame>();
            }

            if (ProcessCameraFrames && cameraMotion != null) {
                cameraFrames = CreateCameraFrames(cameraMotion, FixedFov);
            } else {
                cameraFrames = Array.Empty<VmdCameraFrame>();
            }

            if (ProcessFacialFrames && baseScenario != null && facialExpr != null) {
                var lipFrames = CreateLipSyncFrames(baseScenario, songPosition);
                var exprFrames = CreateFacialExpressionFrames(facialExpr, songPosition);
                var allFrames = new List<VmdFacialFrame>(lipFrames);
                allFrames.AddRange(exprFrames);

                facialFrames = allFrames;
            } else {
                facialFrames = Array.Empty<VmdFacialFrame>();
            }

            if (ProcessLightFrames && baseScenario != null) {
                lightFrames = CreateLightFrames(baseScenario);
            } else {
                lightFrames = Array.Empty<VmdLightFrame>();
            }

            const string modelName = "MODEL_00";

            var vmd = new VmdMotion(modelName, boneFrames, facialFrames, cameraFrames, lightFrames, null);

            return vmd;
        }

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdLightFrame> CreateLightFrames([NotNull] ScenarioObject scenarioObject) {
            var lightFrameList = new List<VmdLightFrame>();
            var lightControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.LightColor).ToArray();

            foreach (var lightControl in lightControls) {
                var n = (int)((float)lightControl.AbsoluteTime * 60.0f);
                int frameIndex;

                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    frameIndex = n / 2;
                } else {
                    frameIndex = n;
                }

                var frame = new VmdLightFrame(frameIndex);

                frame.Position = new Vector3(0.5f, -1f, -0.5f);

                var c = lightControl.Color;
                frame.Color = new Vector3(c.R, c.G, c.B);

                lightFrameList.Add(frame);
            }

            return lightFrameList;
        }

    }
}
