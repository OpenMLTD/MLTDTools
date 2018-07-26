using System.Runtime.CompilerServices;
using UnityStudio.UnityEngine;

namespace MillionDanceView.Extensions {
    internal static class Vector3Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 ToOpenTK(this Vector3 vector) {
            return new OpenTK.Vector3(vector.X, vector.Y, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Vector3 FixCoordSystem(this OpenTK.Vector3 vector) {
            return new OpenTK.Vector3(-vector.X, vector.Y, vector.Z);
        }

    }
}
