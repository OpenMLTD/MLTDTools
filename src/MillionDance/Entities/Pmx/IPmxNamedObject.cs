using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public interface IPmxNamedObject {

        [NotNull]
        string Name { get; }

        [NotNull]
        string NameEnglish { get; }

    }
}
