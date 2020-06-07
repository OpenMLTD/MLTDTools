using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class ScenarioObject {

        [NotNull, ItemNotNull]
        [ScriptableObjectProperty]
        public EventScenarioData[] Scenario { get; set; }

        [NotNull, ItemNotNull]
        [ScriptableObjectProperty(Name = "texs")]
        public TexTargetName[] Textures { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "ap_st")]
        public EventScenarioData ApSt { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "ap_pose")]
        public EventScenarioData ApPose { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "ap_end")]
        public EventScenarioData ApEnd { get; set; }

        // Appeared in newer versions
        [CanBeNull]
        [ScriptableObjectProperty(Name = "ap2_st")]
        public EventScenarioData Ap2St { get; set; }

        // Appeared in newer versions
        [CanBeNull]
        [ScriptableObjectProperty(Name = "ap2_pose")]
        public EventScenarioData Ap2Pose { get; set; }

        // Appeared in newer versions
        [CanBeNull]
        [ScriptableObjectProperty(Name = "ap2_end")]
        public EventScenarioData Ap2End { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "fine_ev")]
        public EventScenarioData FineEvent { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "EyeTexMap")]
        public object[] EyeTextureMap { get; set; }

    }
}
