using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxMorph : IPmxNamedObject {

        internal PmxMorph() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        public int Panel { get; internal set; }

        public MorphOffsetKind OffsetKind { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBaseMorph> Offsets { get; internal set; } = Array.Empty<PmxBaseMorph>();

    }
}
