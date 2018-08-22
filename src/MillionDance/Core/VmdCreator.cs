using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Pmx;
using MillionDance.Entities.Vmd;
using MillionDance.Utilities;
using OpenTK;
using UnityStudio.UnityEngine.Animation;

namespace MillionDance.Core {
    public sealed partial class VmdCreator {

        public bool ProcessBoneFrames { get; set; } = true;

        public bool ProcessCameraFrames { get; set; } = true;

        public bool ProcessFacialFrames { get; set; } = true;

        public bool ProcessLightFrames { get; set; } = true;

        [NotNull]
        public VmdMotion CreateFrom([CanBeNull] CharacterImasMotionAsset bodyMotion, [CanBeNull] Avatar avatar, [CanBeNull] PmxModel mltdPmxModel,
            [CanBeNull] CharacterImasMotionAsset cameraMotion,
            [CanBeNull] ScenarioObject scenarioObject, int songPosition) {
            IReadOnlyList<VmdBoneFrame> boneFrames;
            IReadOnlyList<VmdCameraFrame> cameraFrames;
            IReadOnlyList<VmdFacialFrame> facialFrames;
            IReadOnlyList<VmdLightFrame> lightFrames;

            if (ProcessBoneFrames && (bodyMotion != null && avatar != null && mltdPmxModel != null)) {
                boneFrames = CreateBoneFrames(bodyMotion, avatar, mltdPmxModel);
            } else {
                boneFrames = EmptyArray.Of<VmdBoneFrame>();
            }

            if (ProcessCameraFrames && cameraMotion != null) {
                cameraFrames = CreateCameraFrames(cameraMotion);
            } else {
                cameraFrames = EmptyArray.Of<VmdCameraFrame>();
            }

            if (ProcessFacialFrames && scenarioObject != null) {
                facialFrames = CreateFacialFrames(scenarioObject, songPosition);
            } else {
                facialFrames = EmptyArray.Of<VmdFacialFrame>();
            }

            if (ProcessLightFrames && scenarioObject != null) {
                lightFrames = CreateLightFrames(scenarioObject);
            } else {
                lightFrames = EmptyArray.Of<VmdLightFrame>();
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
