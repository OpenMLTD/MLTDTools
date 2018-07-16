using OpenTK;

namespace MillionDanceView.ObjectGL {
    public sealed class Camera {

        public Camera() {
            Position = new Vector3(0, 1, 0);
            Up = new Vector3(0, 1, 0);
            Target = new Vector3(0, 0, 0);

            Update();
        }

        public Vector3 Position { get; set; }

        public Vector3 Up { get; set; }

        public Vector3 Target { get; set; }

        public Matrix4 ViewMatrix => _viewMatrix;

        public void Update() {
            _viewMatrix = Matrix4.LookAt(Position, Target, Up);
        }

        private Matrix4 _viewMatrix;

    }
}
