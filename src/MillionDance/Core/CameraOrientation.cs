using System;
using System.Runtime.CompilerServices;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    internal static class CameraOrientation {

        // OpenGL Euler angles application order: X-Y-Z (forward=+Z)
        // MMD Euler angles application order: Y-X-Z (forward=+Z)
        // EuclideanSpace Euler angles application order: Y-Z-X (forward=+X)
        public static Vector3 ComputeMmdOrientation(in Quaternion rawLookAt, float rotationZDeg) {
            var result = ComputeOrientation(in rawLookAt);

            // Unity (D3D) is left-handed, MMD (OpenGL) is right-handed
            result.Z = -MathHelper.DegreesToRadians(rotationZDeg);

            return result;
        }

        // https://stackoverflow.com/a/51170230
        public static Quaternion QuaternionLookAt(in Vector3 position, in Vector3 target, in Vector3 up) {
            // Global "forward" = +Z
            var front = Vector3.UnitZ;

            var forward = Vector3.Normalize(target - position);

            var rotAxis = Vector3.Cross(forward, front);

            if (rotAxis.LengthSquared.Equals(0)) {
                rotAxis = up;
            } else {
                rotAxis = rotAxis.Normalized();
            }

            var dot = Vector3.Dot(forward, front);
            var rotAngle = (float)Math.Acos(dot);

            return Quaternion.FromAxisAngle(rotAxis, rotAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 ComputeOrientation(in Quaternion rawLookAt) {
            var result = QuaternionToEuler(in rawLookAt, RotationSequence.Yxz);

            result.X = -result.X;
            result.Y = result.Y + MathHelper.Pi;

            return result;
        }

        // http://bediyap.com/programming/convert-quaternion-to-euler-rotations/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 QuaternionToEuler(in Quaternion q, RotationSequence sequence) {
            var result = new Vector3();

            switch (sequence) {
                case RotationSequence.Xyz: {
                    (result.Z, result.Y, result.X) = RotateThreeAxes(
                        -2 * (q.Y * q.Z - q.W * q.X),
                        q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z,
                        2 * (q.X * q.Z + q.W * q.Y),
                        -2 * (q.X * q.Y - q.W * q.Z),
                        q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

                    break;
                }
                case RotationSequence.Yxz: {
                    (result.Z, result.X, result.Y) = RotateThreeAxes(
                        2 * (q.X * q.Z + q.W * q.Y),
                        q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z,
                        -2 * (q.Y * q.Z - q.W * q.X),
                        2 * (q.X * q.Y + q.W * q.Z),
                        q.W * q.W - q.X * q.X + q.Y * q.Y - q.Z * q.Z);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(sequence), sequence, null);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (float, float, float) RotateThreeAxes(float r11, float r12, float r21, float r31, float r32) {
            var r0 = (float)Math.Atan2(r31, r32);
            var r1 = (float)Math.Asin(r21);
            var r2 = (float)Math.Atan2(r11, r12);

            return (r0, r1, r2);
        }

        private enum RotationSequence {

            Yxz = 0,

            Xyz = 1,

        }

    }
}
