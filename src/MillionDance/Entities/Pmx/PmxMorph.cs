using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDance.Utilities;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxMorph : IPmxNamedObject {

        internal PmxMorph() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        public int Panel { get; internal set; }

        public MorphOffsetKind OffsetKind { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBaseMorph> Offsets { get; internal set; } = EmptyArray.Of<PmxBaseMorph>();

    }
}
