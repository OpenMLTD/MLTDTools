using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public abstract class PmxBaseBody : IPmxNamedObject {

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        public int MaterialIndex { get; internal set; }

        public int GroupIndex { get; internal set; }

        [NotNull]
        public PmxBodyPassGroup PassGroup { get; internal set; }

    }
}
