using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class TexTargetName {

        public int Target { get; set; }

        [NotNull]
        public string Name { get; set; }

    }
}
