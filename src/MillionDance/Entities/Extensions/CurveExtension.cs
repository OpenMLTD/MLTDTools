using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDance.Entities.Mltd;

namespace MillionDance.Entities.Extensions {
    internal static class CurveExtension {

        public static PropertyType GetPropertyType([NotNull] this Curve curve) {
            var index = -1;

            for (var i = 0; i < curve.Attributes.Length; ++i) {
                if (curve.Attributes[i].StartsWith("property_type ")) {
                    index = i;
                    break;
                }
            }

            if (index < 0) {
                throw new KeyNotFoundException();
            }

            var s = curve.Attributes[index].Substring(14); // - "property_type "

            switch (s) {
                case "General":
                    return PropertyType.General;
                case "AngleX":
                    return PropertyType.AngleX;
                case "AngleY":
                    return PropertyType.AngleY;
                case "AngleZ":
                    return PropertyType.AngleZ;
                case "PositionX":
                    return PropertyType.PositionX;
                case "PositionY":
                    return PropertyType.PositionY;
                case "PositionZ":
                    return PropertyType.PositionZ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(s), s, null);
            }
        }

        public static KeyType GetKeyType([NotNull] this Curve curve) {
            var index = -1;

            for (var i = 0; i < curve.Attributes.Length; ++i) {
                if (curve.Attributes[i].StartsWith("key_type ")) {
                    index = i;
                    break;
                }
            }

            if (index < 0) {
                throw new KeyNotFoundException();
            }

            var s = curve.Attributes[index].Substring(9); // - "key_type "

            switch (s) {
                case "Const":
                    return KeyType.Const;
                case "Discreate": // This typo exists in MLTD, not my fault.
                    return KeyType.Discrete;
                case "FullFrame":
                    return KeyType.FullFrame;
                case "FCurve":
                    return KeyType.FCurve;
                default:
                    throw new ArgumentOutOfRangeException(nameof(s), s, null);
            }
        }

        public static string GetPropertyName([NotNull] this Curve curve) {
            var index = -1;

            for (var i = 0; i < curve.Attributes.Length; ++i) {
                if (curve.Attributes[i].StartsWith("property_name ")) {
                    index = i;
                    break;
                }
            }

            if (index < 0) {
                throw new KeyNotFoundException();
            }

            return curve.Attributes[index].Substring(14); // - "property_name "
        }

    }
}
