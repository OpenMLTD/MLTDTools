using Imas.Data.Serialized;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Extensions {
    internal static class ScenarioObjectExtensions {

        public static bool HasFacialExpressionEvents([NotNull] this ScenarioObject scrobj) {
            foreach (var obj in scrobj.Scenario) {
                if (obj.Type == ScenarioDataType.FacialExpression) {
                    return true;
                }
            }

            return false;
        }

        public static bool HasFormationChangeEvents([NotNull] this ScenarioObject scrobj) {
            foreach (var obj in scrobj.Scenario) {
                if (obj.Type == ScenarioDataType.FormationChange) {
                    return true;
                }
            }

            return false;
        }

        public static bool HasSeekFrameEvents([NotNull] this ScenarioObject scrobj) {
            foreach (var obj in scrobj.Scenario) {
                if (obj.Type == ScenarioDataType.DanceAnimationSeekFrame) {
                    return true;
                }
            }

            return false;
        }

    }
}
