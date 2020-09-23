using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;

namespace OpenMLTD.MillionDance {
    internal sealed class MainWorkerInputParams {

        public bool GenerateModel { get; set; }

        public bool GenerateCharacterMotion { get; set; }

        public bool GenerateLipSync { get; set; }

        public bool GenerateFacialExpressions { get; set; }

        public bool GenerateCameraMotion { get; set; }

        public string InputHead { get; set; }

        public string InputBody { get; set; }

        public string InputDance { get; set; }

        public string InputScenario { get; set; }

        public string InputCamera { get; set; }

        public string OutputModel { get; set; }

        public string OutputCharacterAnimation { get; set; }

        public string OutputLipSync { get; set; }

        public string OutputFacialExpressions { get; set; }

        public string OutputCamera { get; set; }

        public MotionFormat MotionSource { get; set; }

        public bool ScalePmx { get; set; }

        public bool ConsiderIdolHeight { get; set; }

        public float IdolHeight { get; set; }

        public bool TranslateBoneNames { get; set; }

        public bool AppendLegIkBones { get; set; }

        public bool FixCenterBones { get; set; }

        public bool ConvertBindingPose { get; set; }

        public bool AppendEyeBones { get; set; }

        public bool HideUnityGeneratedBones { get; set; }

        public bool TranslateFacialExpressionNames { get; set; }

        public bool ImportPhysics { get; set; }

        public bool ApplyGameStyledToon { get; set; }

        public int SkinToonNumber { get; set; }

        public int ClothesToonNumber { get; set; }

        public bool AddHairHighlights { get; set; }

        public bool AddEyesHighlights { get; set; }

        public bool TransformTo30Fps { get; set; }

        public bool ScaleVmd { get; set; }

        public bool UseMvdForCamera { get; set; }

        public uint FixedFov { get; set; }

        public int MotionNumber { get; set; }

        public int FormationNumber { get; set; }

        public string FacialExpressionMappingFilePath { get; set; }

        public AppealType AppealType { get; set; }

        public string ExternalDanceAppealFile { get; set; }

        public FallbackFacialExpressionSource PreferredFacialExpressionSource { get; set; }

        public bool IgnoreSingControl { get; set; }

        [CanBeNull]
        public int? DesiredCameraNumber { get; set; }

        public enum FallbackFacialExpressionSource {

            Landscape = 0,

            Portrait = 1,

        }

    }
}
