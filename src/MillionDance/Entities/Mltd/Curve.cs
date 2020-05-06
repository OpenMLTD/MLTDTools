using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    // True name: Imas.Curve
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention), ThrowOnUnmatched = false, PopulationStrategy = PopulationStrategy.OptIn)]
    public sealed class Curve {

        [ScriptableObjectProperty]
        public string Path { get; set; }

        [ScriptableObjectProperty(Name = "attribs")]
        public string[] Attributes { get; set; }

        [ScriptableObjectProperty]
        public float[] Values { get; set; }

    }
}
