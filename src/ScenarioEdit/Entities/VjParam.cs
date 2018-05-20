using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class VjParam {

        internal VjParam() {
        }

        public bool Use { get; set; }

        [MonoBehaviourProperty(Name = "renderTex")]
        public bool RenderTex { get; set; }

        [MonoBehaviourProperty(Name = "col")]
        public int Column { get; set; }

        public int Row { get; set; }

        public int Begin { get; set; }

        public int Speed { get; set; }

        public ColorRGBA Color { get; set; }

    }
}
