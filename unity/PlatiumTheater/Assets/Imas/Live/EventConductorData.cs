using System;

namespace Imas.Live {
    [Serializable]
    public class EventConductorData {

        public double absTime;

        public bool selected;

        public long tick;

        public int measure;

        public int beat;

        public int track;

        public double tempo;

        public int tsigNumerator;

        public int tsigDemoninator;

        public string marker = string.Empty;

    }
}
