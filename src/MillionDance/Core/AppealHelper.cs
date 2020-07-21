using System.Linq;
using Imas.Data.Serialized;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal static class AppealHelper {

        public static AppealTimeInfo CollectAppealTimeInfo([NotNull] ScenarioObject scenario) {
            var startEvent = scenario.Scenario.Single(ev => ev.Type == ScenarioDataType.AppealStart);
            var poseEvent = scenario.Scenario.Single(ev => ev.Type == ScenarioDataType.AppealPose);
            var endEvent = scenario.Scenario.Single(ev => ev.Type == ScenarioDataType.AppealEnd);

            var startFrame = (int)(startEvent.AbsoluteTime * FrameRate.Mltd);
            var poseFrame = (int)(poseEvent.AbsoluteTime * FrameRate.Mltd);
            var endFrame = (int)(endEvent.AbsoluteTime * FrameRate.Mltd);

            return new AppealTimeInfo(startFrame, poseFrame, endFrame);
        }

        public readonly struct AppealTimeInfo {

            internal AppealTimeInfo(int start, int pose, int end) {
                StartFrame = start;
                PoseFrame = pose;
                EndFrame = end;
            }

            public readonly int StartFrame;

            public readonly int PoseFrame;

            public readonly int EndFrame;

            public static readonly AppealTimeInfo Null = new AppealTimeInfo(-1, -1, -1);

        }

    }
}
