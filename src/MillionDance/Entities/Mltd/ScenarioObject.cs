using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace MillionDance.Entities.Mltd {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class ScenarioObject {

        public EventScenarioData[] Scenario { get; set; }

        [MonoBehaviourProperty(Name = "texs")]
        public TexTargetName[] Textures { get; set; }

        [MonoBehaviourProperty(Name = "ap_st")]
        public EventScenarioData ApSt { get; set; }

        [MonoBehaviourProperty(Name = "ap_post")]
        public EventScenarioData ApPose { get; set; }

        [MonoBehaviourProperty(Name = "ap_end")]
        public EventScenarioData ApEnd { get; set; }

        [MonoBehaviourProperty(Name = "fine_ev")]
        public EventScenarioData FineEvent { get; set; }

    }
}
