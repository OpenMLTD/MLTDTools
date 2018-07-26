using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MillionDanceView.ObjectGL;
using MillionDanceView.Programs;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector3 = OpenTK.Vector3;

namespace MillionDanceView.Internal {
    public sealed class BoneDebugger : DisposableBase, IUpdateable, IDrawable {

        public BoneDebugger([NotNull, ItemNotNull] IReadOnlyList<BoneNode> boneList, [NotNull] SimpleColor simpleColor) {
            _simpleColor = simpleColor;
            _boneList = boneList;

            var vertexBuffer = new VertexBuffer();
            var indexBuffer = new IndexBuffer();

            var boneCount = boneList.Count;

            var vertices = new Vector3[boneCount];

            for (var i = 0; i < boneCount; ++i) {
                vertices[i] = boneList[i].CurrentPosition;
            }

            _vertices = vertices;

            var indices = new uint[(boneCount - 1) * 2];
            var c = 0;

            foreach (var bone in boneList) {
                var parent = bone.Parent;

                if (parent == null) {
                    continue;
                }

                indices[c * 2] = (uint)parent.Index;
                indices[c * 2 + 1] = (uint)bone.Index;

                ++c;

                if (c >= boneCount - 1) {
                    break;
                }
            }

            vertexBuffer.BufferData(vertices, BufferUsageHint.StreamDraw);
            indexBuffer.BufferData(indices, BufferUsageHint.StaticDraw);

            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
        }

        public void Update() {
            if (!Enabled) {
                return;
            }

            for (var i = 0; i < _boneList.Count; ++i) {
                _vertices[i] = _boneList[i].CurrentPosition;
            }

            _vertexBuffer.BufferData(_vertices, BufferUsageHint.StreamDraw);
        }

        public void Draw() {
            if (!Visible) {
                return;
            }

            var originalColor = _simpleColor.Color;

            _simpleColor.Color = new Vector4(1, 0, 0, 0.5f);

            _vertexBuffer.Activate();
            _indexBuffer.Activate();

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);

            var lineCount = _indexBuffer.ItemCount / 2;

            GL.DrawElements(BeginMode.Lines, lineCount * 2, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _simpleColor.Color = originalColor;
        }

        public bool Enabled { get; set; } = true;

        public bool Visible { get; set; } = false;

        protected override void Dispose(bool disposing) {
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;

            _indexBuffer?.Dispose();
            _indexBuffer = null;

            base.Dispose(disposing);
        }

        private readonly Vector3[] _vertices;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private readonly IReadOnlyList<BoneNode> _boneList;

        private readonly SimpleColor _simpleColor;

    }
}
