using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class TexTargetName {

        public int Target { get; set; }

        public string Name { get; set; }

    }
}
