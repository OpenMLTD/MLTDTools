using System;
using OpenTK;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class RotationDebug {

        public static Vector3 DecomposeDeg(this Quaternion q) {
            return DecomposeRad(q) * Rad2Deg;
        }

        // Based on:
        // https://ipfs.io/ipfs/QmXoypizjW3WknFiJnKLwHCnL72vedxjQkDDP1mXWo6uco/wiki/Conversion_between_quaternions_and_Euler_angles.html
        // (Different from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles)
        public static Vector3 DecomposeRad(this Quaternion q) {
            var sinr = 2 * (q.W * q.X - q.Y * q.Z);
            var cosr = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            var roll = (float)Math.Atan2(sinr, cosr);

            var sinp = 2 * (q.X * q.Z + q.W * q.Y);
            float pitch;

            if (Math.Abs(sinp) >= 1) {
                pitch = Math.Sign(sinp) * MathHelper.PiOver2;
            } else {
                pitch = (float)Math.Asin(sinp);
            }

            var siny = 2 * (q.W * q.Z - q.X * q.Y);
            var cosy = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            var yaw = (float)Math.Atan2(siny, cosy);

            return new Vector3(roll, pitch, yaw);
        }

        private const float Rad2Deg = 180 / MathHelper.Pi;

    }
}
