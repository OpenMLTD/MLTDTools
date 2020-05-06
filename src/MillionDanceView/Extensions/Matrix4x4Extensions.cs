using AssetStudio;
using OpenTK;

namespace OpenMLTD.MillionDance.Viewer.Extensions {
    internal static class Matrix4x4Extensions {

        public static Matrix4 ToOpenTK(this Matrix4x4 m) {
            return new Matrix4(
                m.M00, m.M01, m.M02, m.M03,
                m.M10, m.M11, m.M12, m.M13,
                m.M20, m.M21, m.M22, m.M23,
                m.M30, m.M31, m.M32, m.M33
            );
        }

    }
}
