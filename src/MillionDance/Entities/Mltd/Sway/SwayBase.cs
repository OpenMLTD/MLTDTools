using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Mltd.Sway {
    public abstract class SwayBase {

        internal SwayBase() {
        }

        [NotNull]
        public string Path { get; internal set; } = string.Empty;

    }
}
