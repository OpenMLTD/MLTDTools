using System;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mltd;

namespace OpenMLTD.MillionDance.Entities.Extensions {
    internal static class KeyTypeExtension {

        [NotNull]
        public static string ToAttributeTextFast(this KeyType k) {
            switch (k) {
                case KeyType.Const:
                    return "key_type Const";
                case KeyType.Discrete:
                    return "key_type Discreate"; // This typo exists in MLTD, not my fault.
                case KeyType.FullFrame:
                    return "key_type FullFrame";
                case KeyType.FCurve:
                    return "key_type FCurve";
                default:
                    throw new ArgumentOutOfRangeException(nameof(k), k, null);
            }
        }

    }
}
