using System;

namespace OpenMLTD.AllStarsTheater {
    public static class MathHelper {

        public static readonly Random Random = new Random();

        public static string GetHumanReadableFileSize(long byteCount) {
            if (byteCount == 0) {
                return "0 B";
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString("0.#") + SizeSuffixes[place];
        }

        private static readonly string[] SizeSuffixes = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };

    }
}
