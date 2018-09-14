using System.Runtime.CompilerServices;

namespace OpenMLTD.MillionDance.Extensions {
    internal static class QuaternionExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Quaternion FixUnityToOpenTK(this OpenTK.Quaternion q) {
            return new OpenTK.Quaternion(-q.X, q.Y, -q.Z, q.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Quaternion FixOpenTKToVmd(this OpenTK.Quaternion q) {
            //return new OpenTK.Quaternion(-q.X, q.Y, q.Z, q.W);
            return q;
        }

    }
}
