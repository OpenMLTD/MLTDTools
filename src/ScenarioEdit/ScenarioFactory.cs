using System;
using Imas.Data.Serialized;
using JetBrains.Annotations;

namespace OpenMLTD.ScenarioEdit {
    public sealed class ScenarioFactory {

        [NotNull]
        public EventScenarioData CreateShowSongTitle(long tick, int measure, int beat, int track) {
            var ev = new EventScenarioData();
            ev.AbsoluteTime = CalculateAbsoluteTime(tick);
            ev.Tick = tick;
            ev.Measure = measure;
            ev.Beat = beat;
            ev.Track = track;
            ev.Type = ScenarioDataType.ShowSongTitle;

            return ev;
        }

        [NotNull]
        public EventScenarioData CreateHideSongTitle(long tick, int measure, int beat, int track) {
            var ev = new EventScenarioData();
            ev.AbsoluteTime = CalculateAbsoluteTime(tick);
            ev.Tick = tick;
            ev.Measure = measure;
            ev.Beat = beat;
            ev.Track = track;
            ev.Type = ScenarioDataType.HideSongTitle;

            return ev;
        }

        private static double CalculateAbsoluteTime(long tick) {
            throw new NotImplementedException();
        }

    }
}
