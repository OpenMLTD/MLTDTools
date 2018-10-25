using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance {
    internal static class GlobalizationHelper {

        /// <summary>
        /// The uniformed culture used for globalization in this application.
        /// It is the user's UI culture.
        /// </summary>
        [NotNull]
        public static CultureInfo Culture => _uiLocale ?? (_uiLocale = CultureInfo.CurrentUICulture);

        [CanBeNull]
        private static CultureInfo _uiLocale;

    }
}
