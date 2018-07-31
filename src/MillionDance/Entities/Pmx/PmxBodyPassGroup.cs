using JetBrains.Annotations;

namespace MillionDance.Entities.Pmx {
    public sealed class PmxBodyPassGroup {

        private PmxBodyPassGroup() {
        }

        public const int FlagsCount = 16;

        [NotNull]
        public bool[] Flags { get; } = new bool[FlagsCount];

        public static PmxBodyPassGroup FromFlagBits(ushort bits) {
            var g = new PmxBodyPassGroup();

            for (var i = 0; i < FlagsCount; ++i) {
                g.Flags[i] = (bits & (1 << i)) != 0;
            }

            return g;
        }

        public ushort ToFlagBits() {
            ushort result = 0;

            for (var i = 0; i < FlagsCount; ++i) {
                result |= (ushort)(Flags[i] ? (1 << i) : 0);
            }

            return result;
        }

    }
}

