using System;
using System.Runtime.CompilerServices;

namespace OpenMLTD.MiriTore {
    public static class MathUtilities {

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float v, float min, float max) {
            return v < min ? min : (v > max ? max : v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int v, int min, int max) {
            return v < min ? min : (v > max ? max : v);
        }

        private static readonly string[] SizeSuffixes = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };

    }
}
