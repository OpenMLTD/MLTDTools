using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public struct Vector4 {

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

    }
}
