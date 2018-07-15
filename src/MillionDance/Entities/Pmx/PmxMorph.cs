using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxMorph : IPmxNamedObject {

        internal PmxMorph() {
        }

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        public int Panel { get; internal set; }

        public MorphOffsetKind OffsetKind { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBaseMorph> Offsets { get; internal set; }

    }
}
