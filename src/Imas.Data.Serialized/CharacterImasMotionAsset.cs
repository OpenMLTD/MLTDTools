using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace Imas.Data.Serialized {
    // True name: Imas.CharacterImasMotionAsset
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention), ThrowOnUnmatched = false, PopulationStrategy = PopulationStrategy.OptIn)]
    public sealed class CharacterImasMotionAsset {

        [ScriptableObjectProperty]
        public string Kind { get; set; }

        [ScriptableObjectProperty(Name = "attribs")]
        public object[] Atrributes { get; set; }

        [ScriptableObjectProperty(Name = "time_length")]
        public float Duration { get; set; }

        [ScriptableObjectProperty]
        public string Date { get; set; }

        [ScriptableObjectProperty]
        public Curve[] Curves { get; set; }

    }
}
