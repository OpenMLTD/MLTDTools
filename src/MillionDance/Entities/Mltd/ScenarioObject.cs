using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class ScenarioObject {

        public EventScenarioData[] Scenario { get; set; }

        [ScriptableObjectProperty(Name = "texs")]
        public TexTargetName[] Textures { get; set; }

        [ScriptableObjectProperty(Name = "ap_st")]
        public EventScenarioData ApSt { get; set; }

        [ScriptableObjectProperty(Name = "ap_post")]
        public EventScenarioData ApPose { get; set; }

        [ScriptableObjectProperty(Name = "ap_end")]
        public EventScenarioData ApEnd { get; set; }

        [ScriptableObjectProperty(Name = "fine_ev")]
        public EventScenarioData FineEvent { get; set; }

    }
}
