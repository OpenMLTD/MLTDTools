using System.Runtime.CompilerServices;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class Vector3Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 FixUnityToOpenTK(this OpenTK.Vector3 v) {
            return new OpenTK.Vector3(-v.X, v.Y, -v.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 FixOpenTKToVmd(this OpenTK.Vector3 v) {
            //return new OpenTK.Vector3(-v.X, v.Y, -v.Z);
            return v;
        }

    }
}
