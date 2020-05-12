using JetBrains.Annotations;

namespace Imas.Data.Serialized.Sway {
    public abstract class SwayBase {

        internal SwayBase() {
        }

        [NotNull]
        public string Path { get; internal set; } = string.Empty;

    }
}
