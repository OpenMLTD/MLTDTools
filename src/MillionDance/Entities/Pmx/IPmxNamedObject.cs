using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public interface IPmxNamedObject {

        [NotNull]
        string Name { get; }

        [NotNull]
        string NameEnglish { get; }

    }
}
