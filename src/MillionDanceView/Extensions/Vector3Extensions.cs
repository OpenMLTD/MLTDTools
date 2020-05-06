using System.Runtime.CompilerServices;
using OpenTK;

namespace OpenMLTD.MillionDance.Viewer.Extensions {
    internal static class Vector3Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToOpenTK(this AssetStudio.Vector3 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FixCoordSystem(this Vector3 vector) {
            return new Vector3(-vector.X, vector.Y, vector.Z);
        }

    }
}
