using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Vmd {
    public sealed class IkControl {

        internal IkControl([NotNull] string name) {
            Name = name;
        }

        [NotNull]
        public string Name { get; }

        public bool Enabled { get; internal set; }

    }
}
