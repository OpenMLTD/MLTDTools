using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public abstract class PmxBaseBody : IPmxNamedObject {

        protected PmxBaseBody() {
            PassGroup = PmxBodyPassGroup.FromFlagBits(0);
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        public int MaterialIndex { get; internal set; }

        public int GroupIndex { get; internal set; }

        [NotNull]
        public PmxBodyPassGroup PassGroup { get; internal set; }

    }
}
