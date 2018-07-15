using System.Collections.Generic;
using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxNode : IPmxNamedObject {

        internal PmxNode() {
        }

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<NodeElement> Elements { get; internal set; }

        internal bool IsSystemNode { get; set; }

    }
}
