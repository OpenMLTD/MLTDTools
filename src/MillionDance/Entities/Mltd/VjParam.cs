using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace OpenMLTD.MillionDance.Entities.Mltd {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class VjParam {

        internal VjParam() {
        }

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
