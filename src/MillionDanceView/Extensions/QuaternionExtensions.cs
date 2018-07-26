using System.Runtime.CompilerServices;

namespace MillionDanceView.Extensions {
    internal static class QuaternionExtensions {

        /// <summary>
        /// Creates the corresponding <see cref="OpenTK.Quaternion"/> from <see cref="UnityStudio.UnityEngine.Quaternion"/>
        /// </summary>
        /// <param name="q">The original quaternion.</param>
        /// <returns>Created <see cref="OpenTK.Quaternion"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Quaternion ToOpenTK(this UnityStudio.UnityEngine.Quaternion q) {
            return new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        /// <summary>
        /// Fixes the differences between Unity's coordinate system and OpenTK's coordinate system.
        /// </summary>
        /// <param name="q">The quaternion to fix.</param>
        /// <remarks>
        /// Components of the input quaternion should exactly match with Unity's original values.
        /// For example, when the quaterion is (X=0, Y=0.8, Z=0.6, W=0.5) in Unity, you should pass in a <see cref="OpenTK.Quaternion"/>
        /// with (X=0, Y=0.8, Z=0.6, W=0.5).
        /// </remarks>
        /// <returns>Fixed quaternion whose values can be used in OpenTK.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OpenTK.Quaternion FixCoordSystem(this OpenTK.Quaternion q) {
            return new OpenTK.Quaternion(-q.X, -q.Y, q.Z, q.W);
        }

    }
}
