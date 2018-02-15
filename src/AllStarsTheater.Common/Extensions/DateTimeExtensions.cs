using System;
using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.AllStarsTheater.Extensions {
    public static class DateTimeExtensions {

        // Used in server communications.
        // Mon, 01 Jan 9999 00:00:00 GMT
        [NotNull]
        public static string ToGmtString(this DateTime dateTime) {
            return dateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", UsCulture.Value);
        }

        // 2018-02-10T13:06:21+0000
        [NotNull]
        public static string ToMltdJsonString(this DateTime dateTime) {
            var time = dateTime.ToString(@"yyyy\-MM\-dd\THH\:mm\:ss", UsCulture.Value);
            // Remove the ':' (+09:00 -> +0900)
            var tz = dateTime.ToString("zzzz", UsCulture.Value).Replace(":", string.Empty);

            return time + tz;
        }

        private static readonly Lazy<CultureInfo> UsCulture = new Lazy<CultureInfo>(() => CultureInfo.GetCultureInfo("en-US"));

    }
}
