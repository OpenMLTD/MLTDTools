using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Utilities;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxNode : IPmxNamedObject {

        internal PmxNode() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        [NotNull, ItemNotNull]
        public IReadOnlyList<NodeElement> Elements { get; internal set; } = EmptyArray.Of<NodeElement>();

        internal bool IsSystemNode { get; set; }

    }
}
