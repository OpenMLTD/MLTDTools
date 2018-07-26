using System.Runtime.InteropServices;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;

namespace MillionDanceView.ObjectGL {
    public sealed class VertexBuffer : DisposableBase, IGLObject {

        public VertexBuffer() {
            _buffer = GL.GenBuffer();
        }

        public void Activate() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
        }

        public void BufferData<T>(T data, BufferUsageHint usage)
            where T : struct {
            EnsureNotDisposed();

            var dataSize = Marshal.SizeOf(typeof(T));

            Activate();
            GL.BufferData(BufferTarget.ArrayBuffer, dataSize, ref data, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void BufferData<T>([NotNull] T[] data, BufferUsageHint usage)
            where T : struct {
            EnsureNotDisposed();

            var dataSize = Marshal.SizeOf(typeof(T)) * data.Length;

            Activate();
            GL.BufferData(BufferTarget.ArrayBuffer, dataSize, data, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public int ObjectId => _buffer;

        protected override void Dispose(bool disposing) {
            GL.DeleteBuffer(_buffer);
            _buffer = 0;

            base.Dispose(disposing);
        }

        private int _buffer;

    }
}
