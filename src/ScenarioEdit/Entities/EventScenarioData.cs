using JetBrains.Annotations;
using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.ScenarioEdit.Entities {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    public sealed class EventScenarioData {

        [MonoBehaviourProperty(Name = "absTime")]
        public double AbsoluteTime { get; set; }

        public bool Selected { get; set; }

        public long Tick { get; set; }

        public int Measure { get; set; }

        public int Beat { get; set; }

        public int Track { get; set; }

        public ScenarioDataType Type { get; set; }

        public int Param { get; set; }

        public int Target { get; set; }

        public long Duration { get; set; }

        /// <summary>
        /// Lyrics text may contain LF (<code>'\n'</code>), so remember to replace it with <see cref="string.Empty"/>.
        /// </summary>
        [MonoBehaviourProperty(Name = "str")]
        public string Lyrics { get; set; }

        public string Info { get; set; }

        public int On { get; set; }

        public int On2 { get; set; }

        [MonoBehaviourProperty(Name = "col")]
        public ColorRGBA Color { get; set; }

        [MonoBehaviourProperty(Name = "col2")]
        public ColorRGBA Color2 { get; set; }

        [MonoBehaviourProperty(Name = "cols")]
        public float[] Colors { get; set; }

        [MonoBehaviourProperty(Name = "tex")]
        public object Texture { get; set; }

        [MonoBehaviourProperty(Name = "texInx")]
        public int TextureIndex { get; set; }

        [MonoBehaviourProperty(Name = "trig")]
        public int Trigger { get; set; }

        public float Speed { get; set; }

        /// <summary>
        /// Idol index, zero-based (0 to 4).
        /// </summary>
        public int Idol { get; set; }

        public bool[] Mute { get; set; }

        public bool Addf { get; set; }

        [MonoBehaviourProperty(Name = "eye_x")]
        public float EyeX { get; set; }

        [MonoBehaviourProperty(Name = "eye_y")]
        public float EyeY { get; set; }

        public Vector4[] Formation { get; set; }

        public bool Appeal { get; set; }

        [MonoBehaviourProperty(Name = "cheeklv")]
        public int CheekLevel { get; set; }

        [MonoBehaviourProperty(Name = "eyeclose")]
        public bool EyeClosed { get; set; }

        public bool Talking { get; set; }

        public bool Delay { get; set; }

        [MonoBehaviourProperty(Name = "clratio")]
        public int[] ColorRatio { get; set; }

        [MonoBehaviourProperty(Name = "clcols")]
        public int[] ColorColumns { get; set; }

        [MonoBehaviourProperty(Name = "camcut")]
        public int CameraCut { get; set; }

        [CanBeNull]
        [MonoBehaviourProperty(Name = "vjparam")]
        public VjParam VjParam { get; set; }

    }
}
