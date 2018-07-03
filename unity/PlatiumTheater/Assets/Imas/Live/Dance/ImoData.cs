using System;

namespace Imas.Live.Dance {
    [Serializable]
    public sealed class ImoData {

        public string kind;

        public string[] attribs = new string[0];

        public float time_length;

        public string date;

        public Curve[] curves;

    }
}
