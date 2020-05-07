using System;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mltd;

namespace OpenMLTD.MillionDance.Entities.Extensions {
    internal static class PropertyTypeExtension {

        [NotNull]
        public static string ToAttributeTextFast(this PropertyType p) {
            switch (p) {
                case PropertyType.General:
                    return "property_type General";
                case PropertyType.AngleX:
                    return "property_type AngleX";
                case PropertyType.AngleY:
                    return "property_type AngleY";
                case PropertyType.AngleZ:
                    return "property_type AngleZ";
                case PropertyType.PositionX:
                    return "property_type PositionX";
                case PropertyType.PositionY:
                    return "property_type PositionY";
                case PropertyType.PositionZ:
                    return "property_type PositionZ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(p), p, null);
            }
        }

    }
}
