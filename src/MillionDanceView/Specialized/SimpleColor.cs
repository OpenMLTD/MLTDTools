using MillionDanceView.ObjectGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MillionDanceView.Specialized {
    public sealed class SimpleColor : Program {

        private SimpleColor() {
        }

        public void SetMatrices(Matrix4 world, Matrix4 view, Matrix4 projection) {
            EnsureNotDisposed();

            Activate();

            //var wvp = projection * view * world;
            var wvp = world * view * projection;

            var loc = GetUniformLocation(Uniform_WorldViewProjection);

            GL.UniformMatrix4(loc, false, ref wvp);
        }

        // ReSharper disable once InconsistentNaming
        private const string Uniform_WorldViewProjection = "uWVP";

    }
}
