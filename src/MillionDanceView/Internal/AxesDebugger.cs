using System;
using JetBrains.Annotations;
using MillionDanceView.ObjectGL;
using MillionDanceView.Programs;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MillionDanceView.Internal {
    internal sealed class AxesDebugger : DisposableBase, IUpdateable, IDrawable {

        internal AxesDebugger([NotNull] SimpleColor simpleColor) {
            _simpleColor = simpleColor;

            var vertices = new[] {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1)
            };
            var indices = new uint[] {
                0, 1, 0, 2, 0, 3
            };

            var vertexBuffer = new VertexBuffer();
            var indexBuffer = new IndexBuffer();

            vertexBuffer.BufferData(vertices, BufferUsageHint.StaticDraw);
            indexBuffer.BufferData(indices, BufferUsageHint.StaticDraw);

            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
        }

        public void Update() {
        }

        public void Draw() {
            if (!Visible) {
                return;
            }

            var originalColor = _simpleColor.Color;

            DrawAxis(0, new Vector4(1, 0, 0, 1));
            DrawAxis(1, new Vector4(0, 1, 0, 1));
            DrawAxis(2, new Vector4(0, 0, 1, 1));

            _simpleColor.Color = originalColor;

            void DrawAxis(int startIndex, Vector4 color) {
                _simpleColor.Color = color;

                _vertexBuffer.Activate();
                _indexBuffer.Activate();

                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);

                GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedInt, startIndex * 2 * sizeof(uint));

                GL.DisableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        public bool Enabled { get; set; } = true;

        public bool Visible { get; set; } = true;

        protected override void Dispose(bool disposing) {
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;

            _indexBuffer?.Dispose();
            _indexBuffer = null;

            base.Dispose(disposing);
        }

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private readonly SimpleColor _simpleColor;

    }
}
