using Imas.Data.Serialized;
using JetBrains.Annotations;

namespace OpenMLTD.MLTDTools.Applications.TDFacial.Extensions {
    internal static class ScenarioObjectExtensions {

        public static bool HasFacialExpressionFrames([NotNull] this ScenarioObject scrobj) {
            foreach (var obj in scrobj.Scenario) {
                if (obj.Type == ScenarioDataType.FacialExpression) {
                    return true;
                }
            }

            return false;
        }

    }
}
