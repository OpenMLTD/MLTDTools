using OpenMLTD.MillionDance.Viewer.ObjectGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.Programs {
    public sealed class SimpleColor : Program {

        private SimpleColor() {
            Color = new Vector4(1, 0, 0, 1);
        }

        public void SetMatrices(Matrix4 world, Matrix4 view, Matrix4 projection) {
            EnsureNotDisposed();

            Activate();

            //var wvp = projection * view * world;
            var wvp = world * view * projection;

            var loc = GetUniformLocation(Uniform_WorldViewProjection);

            GL.UniformMatrix4(loc, false, ref wvp);
        }

        public Vector4 Color {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_Color);
                var buf = new float[4];

                GL.GetUniform(ObjectId, loc, buf);

                return new Vector4(buf[0], buf[1], buf[2], buf[3]);
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_Color);

                GL.Uniform4(loc, value);
            }
        }

        // ReSharper disable once InconsistentNaming
        private const string Uniform_WorldViewProjection = "uWVP";
        private const string Uniform_Color = "uColor";

    }
}
