using System.Runtime.CompilerServices;
using OpenTK;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class GeometryHelper {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInTriangle(in Vector2 point, in Vector2 p1, in Vector2 p2, in Vector2 p3) {
            return PointInTriangle(in point, in p1, in p2, in p3, out _, out _);
        }

        // p = p1 + u * (p3 - p1) + v * (p2 - p1)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInTriangle(in Vector2 point, in Vector2 p1, in Vector2 p2, in Vector2 p3, out float u, out float v) {
            // https://blackpawn.com/texts/pointinpoly/default.html
            var v0 = p3 - p1;
            var v1 = p2 - p1;
            var v2 = point - p1;

            var dot00 = Vector2.Dot(v0, v0);
            var dot01 = Vector2.Dot(v0, v1);
            var dot02 = Vector2.Dot(v0, v2);
            var dot11 = Vector2.Dot(v1, v1);
            var dot12 = Vector2.Dot(v1, v2);

            var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Note the last lteq, we also want to handle cases on the edge
            return u >= 0 && v >= 0 && (u + v <= 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float from, float to, float t) {
            return from * (1 - t) + to * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Blerp(float v00, float v10, float v01, float v11, float tx, float ty) {
            return Lerp(Lerp(v00, v10, tx), Lerp(v01, v11, tx), ty);
        }

    }
}
