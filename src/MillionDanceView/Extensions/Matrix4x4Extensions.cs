using OpenTK;
using UnityStudio.UnityEngine;

namespace MillionDanceView.Extensions {
    internal static class Matrix4x4Extensions {

        public static Matrix4 ToOpenTK(this Matrix4x4 m) {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            );
        }

    }
}
