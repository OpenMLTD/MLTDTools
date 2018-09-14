using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.ObjectGL {
    public sealed class VertexArrayObject : DisposableBase {

        public VertexArrayObject() {
            _vao = GL.GenVertexArray();
        }

        public void Activate() {
            EnsureNotDisposed();

            GL.BindVertexArray(_vao);
        }

        public int ObjectId => _vao;

        protected override void Dispose(bool disposing) {
            GL.DeleteVertexArray(_vao);
            _vao = 0;

            base.Dispose(disposing);
        }

        private int _vao;

    }
}
