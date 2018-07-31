#define STATIC_POSE_DEBUG
#undef STATIC_POSE_DEBUG
#define NO_TRANSFORM_MESH
#undef NO_TRANSFORM_MESH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using MillionDance.Entities.Internal;
using MillionDanceView.Extensions;
using MillionDanceView.ObjectGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using UnityStudio.UnityEngine;
using UnityStudio.UnityEngine.Animation;
using Vector3 = OpenTK.Vector3;

namespace MillionDanceView.Internal {
    internal sealed class AnimationRenderer : DisposableBase, IUpdateable, IDrawable {

        public AnimationRenderer([NotNull] Game game, [NotNull] Mesh mesh, [NotNull] Avatar avatar, [NotNull] BodyAnimation animation, [NotNull, ItemNotNull] IReadOnlyList<BoneNode> boneList) {
            _game = game;
            _mesh = mesh;
            _avatar = avatar;
            _animation = animation;
            _boneList = boneList;

            var vertexBuffer = new VertexBuffer();
            var indexBuffer = new IndexBuffer();

            var vertices = new PosNorm[mesh.Vertices.Length];
            var indices = new uint[mesh.Indices.Count];

            for (var k = 0; k < vertices.Length; ++k) {
                vertices[k] = new PosNorm {
                    Position = mesh.Vertices[k].ToOpenTK().FixCoordSystem(),
                    Normal = mesh.Normals[k].ToOpenTK().FixCoordSystem()
                };
            }

            _originalVertices = vertices;
            _vertices = (PosNorm[])vertices.Clone();

            for (var k = 0; k < indices.Length; ++k) {
                indices[k] = mesh.Indices[k];
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

            UpdateVertices(_game.CurrentTime, _originalVertices, _vertices);

            _vertexBuffer.BufferData(_vertices, BufferUsageHint.StreamDraw);
        }

        public void Draw() {
            if (!Visible) {
                return;
            }

            foreach (var subMesh in _mesh.SubMeshes) {
                DrawBuffered(_vertexBuffer, _indexBuffer, subMesh.FirstIndex, subMesh.IndexCount * 3);
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

        private static void DrawBuffered([NotNull] VertexBuffer vertexBuffer, [NotNull] IndexBuffer indexBuffer, uint startIndex, uint elementCount) {
            Debug.Assert(elementCount % 3 == 0);

            vertexBuffer.Activate();
            indexBuffer.Activate();

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), IntPtr.Zero);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), IntPtr.Zero + 3 * sizeof(float));

            GL.DrawElements(BeginMode.Triangles, (int)elementCount * 3, DrawElementsType.UnsignedInt, (int)startIndex * sizeof(uint));

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void UpdateVertices(float currentTime, [NotNull] PosNorm[] origVertices, [NotNull] PosNorm[] vertices) {
            var mesh = _mesh;
            var avatar = _avatar;
            var keyFrames = _animation.KeyFrames;

#if !STATIC_POSE_DEBUG 
            var animatedBoneCount = _animation.BoneCount;
            var firstKeyFrameIndex = -1;
            var lastFirstKeyFrameIndex = 0;

            for (var i = 0; i < keyFrames.Count; i += animatedBoneCount) {
                //if (keyFrames[i].Time >= currentTime) {
                if (keyFrames[i].FrameIndex > _lastFrameIndex) {
                    firstKeyFrameIndex = lastFirstKeyFrameIndex;
                    break;
                }

                lastFirstKeyFrameIndex = i;
            }

            if (firstKeyFrameIndex < 0) {
                // We are at the end of the whole animation.
                return;
            }

            for (var i = firstKeyFrameIndex; i < firstKeyFrameIndex + animatedBoneCount; ++i) {
                var frame = keyFrames[i];
                var altPath = frame.Path.Contains("BODY_SCALE/") ? frame.Path.Replace("BODY_SCALE/", string.Empty) : frame.Path;
                var bone = _boneList.FirstOrDefault(b => b.Path == altPath);

                if (bone == null) {
                    throw new ApplicationException($"Invalid bone path: {altPath}");
                }

                if (frame.HasPositions) {
                    var x = frame.PositionX.Value;
                    var y = frame.PositionY.Value;
                    var z = frame.PositionZ.Value;

                    var t = new Vector3(x, y, z);

                    t = t.FixCoordSystem();

                    bone.LocalPosition = t;
                }

                if (frame.HasRotations) {
                    var q = UnityRotation.EulerDeg(frame.AngleX.Value, frame.AngleY.Value, frame.AngleZ.Value);

                    q = q.FixCoordSystem();

                    bone.LocalRotation = q;
                }
            }
#endif

            foreach (var bone in _boneList) {
                bone.UpdateTransform();
            }

            var vertexCount = vertices.Length;

            for (var i = 0; i < vertexCount; ++i) {
                var vertex = origVertices[i];
                var skin = mesh.Skin[i];

                var m = Matrix4.Zero;
                var activeSkinCount = 0;
                float totalWeight = 0;

                for (var j = 0; j < 4; ++j) {
                    if (skin[j].Weight <= 0) {
                        continue;
                    }

                    ++activeSkinCount;

                    var boneNameHash = mesh.BoneNameHashes[skin[j].BoneIndex];
                    var boneIndex = avatar.FindBoneIndexByNameHash(boneNameHash);
                    var transform = _boneList[boneIndex].SkinMatrix;
                    m = m + transform * skin[j].Weight;
                    totalWeight += skin[j].Weight;
                }

                if (activeSkinCount == 0) {
#if DEBUG
                    Debug.Print("Warning: one skin is not bound to bones.");
#endif
                    continue;
                }

#if DEBUG
                if (Math.Abs(totalWeight - 1.0f) > 0.00001f) {
                    Debug.Print("Warning: weights do not sum up.");
                }
#endif

#if !NO_TRANSFORM_MESH
                vertex.Position = Vector3.TransformPosition(vertex.Position, m);
                vertex.Normal = Vector3.TransformNormal(vertex.Normal, m);
#endif

                vertices[i] = vertex;
            }

            ++_lastFrameIndex;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PosNorm {

            public Vector3 Position;

            public Vector3 Normal;

        }

        private readonly Mesh _mesh;
        private readonly Avatar _avatar;
        private readonly BodyAnimation _animation;
        private readonly IReadOnlyList<BoneNode> _boneList;

        private readonly PosNorm[] _originalVertices;
        private readonly PosNorm[] _vertices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private int _lastFrameIndex = -1;

        private readonly Game _game;

    }
}
