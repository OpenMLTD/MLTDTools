using System;
using System.Diagnostics;
using AssetStudio;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    [DebuggerDisplay("Scenario data (type: {Type}, time: {AbsoluteTime})")]
    public sealed class EventScenarioData {

        [ScriptableObjectProperty(Name = "absTime")]
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
        [ScriptableObjectProperty(Name = "str")]
        public string Lyrics { get; set; } = string.Empty;

        public string Info { get; set; } = string.Empty;

        public int On { get; set; }

        public int On2 { get; set; }

        [ScriptableObjectProperty(Name = "col")]
        public ColorRGBA Color { get; set; }

        [ScriptableObjectProperty(Name = "col2")]
        public ColorRGBA Color2 { get; set; }

        [ScriptableObjectProperty(Name = "cols")]
        public float[] Colors { get; set; } = Array.Empty<float>();

        [ScriptableObjectProperty(Name = "tex")]
        public object Texture { get; set; } = null;

        [ScriptableObjectProperty(Name = "texInx")]
        public int TextureIndex { get; set; }

        [ScriptableObjectProperty(Name = "trig")]
        public int Trigger { get; set; }

        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Idol index, zero-based (0 to 4).
        /// </summary>
        public int Idol { get; set; }

        public bool[] Mute { get; set; } = Array.Empty<bool>();

        public bool Addf { get; set; }

        [ScriptableObjectProperty(Name = "eye_x")]
        public float EyeX { get; set; }

        [ScriptableObjectProperty(Name = "eye_y")]
        public float EyeY { get; set; }

        public Vector4[] Formation { get; set; } = Array.Empty<Vector4>();

        public bool Appeal { get; set; }

        [ScriptableObjectProperty(Name = "cheeklv")]
        public int CheekLevel { get; set; }

        [ScriptableObjectProperty(Name = "eyeclose")]
        public bool EyeClosed { get; set; }

        public bool Talking { get; set; }

        public bool Delay { get; set; }

        [ScriptableObjectProperty(Name = "clratio")]
        public int[] ColorRatio { get; set; } = Array.Empty<int>();

        [ScriptableObjectProperty(Name = "clcols")]
        public int[] ColorColumns { get; set; } = Array.Empty<int>();

        [ScriptableObjectProperty(Name = "camcut")]
        public int CameraCut { get; set; }

        [CanBeNull]
        [ScriptableObjectProperty(Name = "vjparam")]
        public VjParam VjParam { get; set; }

    }
}
