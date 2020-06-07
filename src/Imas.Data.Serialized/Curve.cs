using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace Imas.Data.Serialized {
    // True name: Imas.Curve
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention), ThrowOnUnmatched = false, PopulationStrategy = PopulationStrategy.OptIn)]
    public sealed class Curve {

        [NotNull]
        [ScriptableObjectProperty]
        public string Path { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "attribs")]
        public string[] Attributes { get; set; }

        [NotNull]
        [ScriptableObjectProperty]
        public float[] Values { get; set; }

    }
}
