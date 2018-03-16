using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public struct Vector4 {

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

    }
}
