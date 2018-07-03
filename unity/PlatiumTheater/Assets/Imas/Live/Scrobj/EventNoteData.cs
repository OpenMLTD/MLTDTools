using System;
using JetBrains.Annotations;

namespace Imas.Live.Scrobj {
    [Serializable]
    public sealed class EventNoteData {

        public double absTime;

        public bool selected;

        public long tick;

        public int measure;

        public int beat;

        public int track;

        public int type;

        public float startPosx;

        public float endPosx;

        public float speed;

        public int duration;

        [CanBeNull]
        public PolyPoint[] poly;

        public int endType;

        public double leadTime;

    }
}
