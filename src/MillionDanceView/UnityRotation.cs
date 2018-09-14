using System;
using System.Runtime.CompilerServices;
using OpenMLTD.MillionDance.Viewer.Extensions;
using OpenTK;

namespace OpenMLTD.MillionDance.Viewer {
    internal static class UnityRotation {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion EulerDeg(float x, float y, float z) {
            return EulerDeg(new Vector3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion EulerRad(float x, float y, float z) {
            return EulerRad(new Vector3(x, y, z));
        }

        /// <summary>
        /// Creates a Unity-like quaternion from Euler angles in degrees.
        /// </summary>
        /// <param name="angles">Angles in degrees.</param>
        /// <remarks>
        /// Created quaternion is in Unity's rotation representation. The value must be converted using
        /// <see cref="QuaternionExtensions.FixCoordSystem"/> in order to use in OpenTK.
        /// See https://gist.github.com/aeroson/043001ca12fe29ee911e#gistcomment-2191151 for discussion.
        /// </remarks>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion EulerDeg(Vector3 angles) {
            return EulerRad(angles * Deg2Rad);
        }

        /// <summary>
        /// Creates a Unity-like quaternion from Euler angles in radians.
        /// </summary>
        /// <param name="angles"></param>
        /// <remarks>
        /// Created quaternion is in Unity's rotation representation. The value must be converted using
        /// <see cref="QuaternionExtensions.FixCoordSystem"/> in order to use in OpenTK.
        /// See https://gist.github.com/aeroson/043001ca12fe29ee911e#gistcomment-2191151 for discussion.
        /// </remarks>
        /// <returns></returns>
        public static Quaternion EulerRad(Vector3 angles) {
            var pitchOver2 = angles.Y / 2;
            var yawOver2 = angles.X / 2;
            var rollOver2 = angles.Z / 2;

            var sinPitchOver2 = (float)Math.Sin(pitchOver2);
            var cosPitchOver2 = (float)Math.Cos(pitchOver2);
            var sinYawOver2 = (float)Math.Sin(yawOver2);
            var cosYawOver2 = (float)Math.Cos(yawOver2);
            var sinRollOver2 = (float)Math.Sin(rollOver2);
            var cosRollOver2 = (float)Math.Cos(rollOver2);

            var result = new Quaternion();

            result.X = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2; // confirmed (scc+css)
            result.Y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2; // confirmed (csc-scs)
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2; // confirmed (ccs-ssc)
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2; // confirmed (ccc+sss)

            return result;
        }

        private const float Deg2Rad = MathHelper.Pi / 180;

    }
}
