using System;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxNode : IPmxNamedObject {

        internal PmxNode() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        [NotNull, ItemNotNull]
        public NodeElement[] Elements { get; internal set; } = Array.Empty<NodeElement>();

        internal bool IsSystemNode { get; set; }

    }
}
