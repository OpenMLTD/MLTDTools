using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx.Extensions {
    internal static class PmxPassGroupExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PassAll([NotNull] this PmxBodyPassGroup passGroup) {
            for (var i = 0; i < PmxBodyPassGroup.FlagsCount; ++i) {
                passGroup.Flags[i] = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlockAll([NotNull] this PmxBodyPassGroup passGroup) {
            for (var i = 0; i < PmxBodyPassGroup.FlagsCount; ++i) {
                passGroup.Flags[i] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pass([NotNull] this PmxBodyPassGroup passGroup, int index) {
            if (index < 0 || index >= PmxBodyPassGroup.FlagsCount) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            passGroup.Flags[index] = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Block([NotNull] this PmxBodyPassGroup passGroup, int index) {
            if (index < 0 || index >= PmxBodyPassGroup.FlagsCount) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            passGroup.Flags[index] = true;
        }

    }
}
