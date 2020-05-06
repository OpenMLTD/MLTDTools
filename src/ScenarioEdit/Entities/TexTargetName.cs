using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class TexTargetName {

        public int Target { get; set; }

        public string Name { get; set; }

    }
}
