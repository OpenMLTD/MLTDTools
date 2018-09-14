using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
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
