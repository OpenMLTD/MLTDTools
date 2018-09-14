using System.Runtime.InteropServices;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.ObjectGL {
    public sealed class IndexBuffer : DisposableBase, IGLObject {

        public IndexBuffer() {
            _vbo = GL.GenBuffer();
        }

        public void Activate() {
            EnsureNotDisposed();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vbo);
        }

        public void BufferData<T>(T data, BufferUsageHint usage)
            where T : struct {
            EnsureNotDisposed();

            var dataSize = Marshal.SizeOf(typeof(T));

            Activate();
            GL.BufferData(BufferTarget.ElementArrayBuffer, dataSize, ref data, usage);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _itemCount = 1;
        }

        public void BufferData<T>([NotNull] T[] data, BufferUsageHint usage)
            where T : struct {
            EnsureNotDisposed();

            var dataSize = Marshal.SizeOf(typeof(T)) * data.Length;

            Activate();
            GL.BufferData(BufferTarget.ElementArrayBuffer, dataSize, data, usage);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _itemCount = data.Length;
        }

        public int ObjectId => _vbo;

        public int ItemCount => _itemCount;

        protected override void Dispose(bool disposing) {
            GL.DeleteBuffer(_vbo);
            _vbo = 0;

            base.Dispose(disposing);
        }

        private int _vbo;
        private int _itemCount;

    }
}
