using System.Runtime.CompilerServices;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class UnityStudioExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector2 ToOpenTK(this UnityStudio.UnityEngine.Vector2 v) {
            return new OpenTK.Vector2(v.X, v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 ToOpenTK(this UnityStudio.UnityEngine.Vector3 v) {
            return new OpenTK.Vector3(v.X, v.Y, v.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector4 ToOpenTK(this UnityStudio.UnityEngine.Vector4 v) {
            return new OpenTK.Vector4(v.X, v.Y, v.Z, v.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Quaternion ToOpenTK(this UnityStudio.UnityEngine.Quaternion q) {
            return new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static OpenTK.Matrix4 ToOpenTK(this UnityStudio.UnityEngine.Matrix4x4 m) {
            return new OpenTK.Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            );
        }

    }
}
