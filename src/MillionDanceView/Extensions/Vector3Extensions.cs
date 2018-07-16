using System.Runtime.CompilerServices;

namespace MillionDanceView.Extensions {
    internal static class Vector3Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 ToOpenTK(this UnityStudio.Unity.Vector3 vector) {
            return new OpenTK.Vector3(vector.X, vector.Y, vector.Z);
        }

    }
}
