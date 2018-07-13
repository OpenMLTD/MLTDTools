using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace MillionDance.Entities.Unity {
    // True name: Imas.Curve
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention), ThrowOnUnmatched = false, PopulationStrategy = PopulationStrategy.OptIn)]
    public sealed class Curve {

        [MonoBehaviourProperty]
        public string Path { get; set; }

        [MonoBehaviourProperty(Name = "attribs")]
        public string[] Attributes { get; set; }

        [MonoBehaviourProperty]
        public float[] Values { get; set; }

    }
}
