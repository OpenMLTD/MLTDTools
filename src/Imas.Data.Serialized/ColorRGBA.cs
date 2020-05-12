using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    // ReSharper disable once InconsistentNaming
    public struct ColorRGBA {

        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public float A { get; set; }

        public override string ToString() {
            return $"ColorRGBA ({R}, {G}, {B}, {A})";
        }

    }
}
