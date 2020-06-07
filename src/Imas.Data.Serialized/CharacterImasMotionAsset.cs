using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace Imas.Data.Serialized {
    // True name: Imas.CharacterImasMotionAsset
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention), ThrowOnUnmatched = false, PopulationStrategy = PopulationStrategy.OptIn)]
    public sealed class CharacterImasMotionAsset {

        [NotNull]
        [ScriptableObjectProperty]
        public string Kind { get; set; }

        [NotNull]
        [ScriptableObjectProperty(Name = "attribs")]
        public object[] Attributes { get; set; }

        [ScriptableObjectProperty(Name = "time_length")]
        public float Duration { get; set; }

        [NotNull]
        [ScriptableObjectProperty]
        public string Date { get; set; }

        [NotNull, ItemNotNull]
        [ScriptableObjectProperty]
        public Curve[] Curves { get; set; }

    }
}
