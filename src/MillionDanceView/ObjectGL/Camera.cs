using System;
using OpenTK;

namespace MillionDanceView.ObjectGL {
    public sealed class Camera {

        public Camera() {
            Position = new Vector3(0, 1, 0);
            Up = new Vector3(0, 1, 0);
            Orientation = new Vector3(MathHelper.Pi, 0, 0);

            Update();
        }

        public Vector3 Position { get; set; }

        public Vector3 Up { get; set; }

        public Vector3 Orientation { get; set; }

        public Matrix4 ViewMatrix => _viewMatrix;

        public void Update() {
            var target = Position + Orientation;
            _viewMatrix = Matrix4.LookAt(Position, target, Up);
        }

        public void LookAtTarget(Vector3 target) {
            var diff = target - Position;

            if (diff == Vector3.Zero) {
                return;
            }

            var length = diff.Length;

            var x = diff.X / length;
            var y = diff.Y / length;
            var z = diff.Z / length;

            var o = new Vector3((float)Math.Asin(x), (float)Math.Asin(y), (float)Math.Asin(z));

            Orientation = o;
        }

        public void Move(float x, float y, float z) {
            var offset = new Vector3();

            var forward = Orientation;
            var right = Vector3.Cross(forward, Up);

            forward.Normalize();
            right.Normalize();

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }

        public void AddRotation(float x, float y) {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            var orientation = Orientation;

            // -180 - +180 (degrees)
            orientation.X = (orientation.X - x) % MathHelper.TwoPi;
            orientation.Y = (orientation.Y + y) % MathHelper.TwoPi;

            Orientation = orientation;
        }

        public static readonly float MoveSpeed = 0.05f;

        public static readonly float MouseSensitivity = 0.01f;

        private Matrix4 _viewMatrix;

    }
}
