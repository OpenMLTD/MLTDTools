using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class VjParam {

        public bool Use { get; set; }

        [ScriptableObjectProperty(Name = "renderTex")]
        public bool RenderTex { get; set; }

        [ScriptableObjectProperty(Name = "col")]
        public int Column { get; set; }

        public int Row { get; set; }

        public int Begin { get; set; }

        public int Speed { get; set; }

        public ColorRGBA Color { get; set; }

    }
}
