using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours {
    /// <summary>
    /// Represents a simple type converter interface. A lightweight version of <see cref="TypeConverter"/>.
    /// </summary>
    public interface ISimpleTypeConverter {

        /// <summary>
        /// Determines whether this converter can handle conversions from specified type.
        /// </summary>
        /// <param name="sourceType">Specified type of input acceptance test.</param>
        /// <returns><see langword="true"/> when the converter accepts the specified type, otherwise <see langword="false"/>.</returns>
        bool CanConvertFrom([NotNull] Type sourceType);

        /// <summary>
        /// Determines whether this converter can handle conversions to specified type.
        /// </summary>
        /// <param name="destinationType">Specified type of output acceptance test.</param>
        /// <returns><see langword="true"/> when the converter accepts the specified type, otherwise <see langword="false"/>.</returns>
        bool CanConvertTo([NotNull] Type destinationType);

        /// <summary>
        /// Converts a value to specified type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceType">Type of the input value.</param>
        /// <param name="destinationType">Required output type.</param>
        /// <returns>Converted value.</returns>
        object ConvertTo([CanBeNull] object value, [NotNull] Type sourceType, [NotNull] Type destinationType);

    }
}
