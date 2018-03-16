using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class TexTargetName {

        public int Target { get; set; }

        public string Name { get; set; }

    }
}
