using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Core {
    internal sealed class ConversionConfig {

        public MotionFormat MotionFormat { get; internal set; }

        // PMX

        public bool ScaleToPmxSize { get; internal set; }

        public bool ApplyPmxCharacterHeight { get; internal set; }

        public bool TranslateBoneNamesToMmd { get; internal set; }

        public bool AppendIKBones { get; internal set; }

        public bool FixMmdCenterBones { get; internal set; }

        public bool FixTdaBindingPose { get; internal set; }

        public bool AppendEyeBones { get; internal set; }

        public bool HideUnityGeneratedBones { get; internal set; }

        public SkeletonFormat SkeletonFormat { get; internal set; }

        public bool TranslateFacialExpressionNamesToMmd { get; internal set; }

        public bool ImportPhysics { get; internal set; }

        // VMD

        public bool Transform60FpsTo30Fps { get; internal set; }

        public bool ScaleToVmdSize { get; internal set; }

        [NotNull]
        public IReadOnlyDictionary<int, IReadOnlyDictionary<string, float>> FacialExpressionMappings { get; internal set; }

        [NotNull]
        private static IReadOnlyDictionary<int, IReadOnlyDictionary<string, float>> ConvertDictionary([NotNull] IReadOnlyDictionary<VmdCreator.FacialExpressionKind, IReadOnlyDictionary<string, float>> dict) {
            var d = new Dictionary<int, IReadOnlyDictionary<string, float>>();

            foreach (var kv in dict) {
                d[(int)kv.Key] = kv.Value;
            }

            return d;
        }

        private static readonly ConversionConfig UseMltdMotion = new ConversionConfig {
            MotionFormat = MotionFormat.Mltd,
            ScaleToPmxSize = true,
            ApplyPmxCharacterHeight = true,
            TranslateBoneNamesToMmd = true,
            AppendIKBones = false,
            FixMmdCenterBones = false,
            FixTdaBindingPose = false,
            AppendEyeBones = false,
            HideUnityGeneratedBones = true,
            SkeletonFormat = SkeletonFormat.Mltd,
            TranslateFacialExpressionNamesToMmd = true,
            ImportPhysics = true,
            Transform60FpsTo30Fps = false,
            ScaleToVmdSize = true,
            FacialExpressionMappings = ConvertDictionary(VmdCreator.DefaultFacialExpressionTable)
        };

        private static readonly ConversionConfig UseMmdMotion = new ConversionConfig {
            MotionFormat = MotionFormat.Mmd,
            ScaleToPmxSize = true,
            ApplyPmxCharacterHeight = true,
            TranslateBoneNamesToMmd = true,
            AppendIKBones = true,
            FixMmdCenterBones = true,
            FixTdaBindingPose = true,
            AppendEyeBones = true,
            HideUnityGeneratedBones = true,
            SkeletonFormat = SkeletonFormat.Mmd,
            TranslateFacialExpressionNamesToMmd = true,
            ImportPhysics = true,
            Transform60FpsTo30Fps = true,
            ScaleToVmdSize = true,
            FacialExpressionMappings = ConvertDictionary(VmdCreator.DefaultFacialExpressionTable)
        };

        public static ConversionConfig Current { get; internal set; } = UseMltdMotion;

    }
}
