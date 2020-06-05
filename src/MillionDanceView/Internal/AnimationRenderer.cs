#define STATIC_POSE_DEBUG
#undef STATIC_POSE_DEBUG
#define NO_TRANSFORM_MESH
#undef NO_TRANSFORM_MESH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using AssetStudio.Extended.CompositeModels;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Viewer.Extensions;
using OpenMLTD.MillionDance.Viewer.ObjectGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.Internal {
    internal sealed class AnimationRenderer : DisposableBase, IUpdateable, IDrawable {

        public AnimationRenderer([NotNull] Game game,
            [NotNull] PrettyMesh bodyMesh, [NotNull] PrettyAvatar bodyAvatar, [NotNull, ItemNotNull] IReadOnlyList<BoneNode> bodyBoneList,
            [NotNull] PrettyMesh headMesh, [NotNull] PrettyAvatar headAvatar, [NotNull, ItemNotNull] IReadOnlyList<BoneNode> headBoneList,
            [NotNull] BodyAnimation animation) {
            _game = game;
            _bodyMesh = bodyMesh;
            _bodyAvatar = bodyAvatar;
            _bodyBoneList = bodyBoneList;
            _headMesh = headMesh;
            _headAvatar = headAvatar;
            _headBoneList = headBoneList;
            _animation = animation;

            #region Body

            var bodyVertexBuffer = new VertexBuffer();
            var bodyIndexBuffer = new IndexBuffer();

            var bodyVertices = new PosNorm[bodyMesh.Vertices.Length];
            var bodyIndices = new uint[bodyMesh.Indices.Length];

            for (var k = 0; k < bodyVertices.Length; ++k) {
                bodyVertices[k] = new PosNorm {
                    Position = bodyMesh.Vertices[k].ToOpenTK().FixCoordSystem(),
                    Normal = bodyMesh.Normals[k].ToOpenTK().FixCoordSystem()
                };
            }

            _originalBodyVertices = bodyVertices;
            _bodyVertices = (PosNorm[])bodyVertices.Clone();

            for (var k = 0; k < bodyIndices.Length; ++k) {
                bodyIndices[k] = bodyMesh.Indices[k];
            }

            bodyVertexBuffer.BufferData(bodyVertices, BufferUsageHint.StreamDraw);
            bodyIndexBuffer.BufferData(bodyIndices, BufferUsageHint.StaticDraw);

            _bodyVertexBuffer = bodyVertexBuffer;
            _bodyIndexBuffer = bodyIndexBuffer;

            #endregion

            #region Head

            var headVertexBuffer = new VertexBuffer();
            var headIndexBuffer = new IndexBuffer();

            var headVertices = new PosNorm[headMesh.Vertices.Length];
            var headIndices = new uint[headMesh.Indices.Length];

            for (var k = 0; k < headVertices.Length; ++k) {
                headVertices[k] = new PosNorm {
                    Position = headMesh.Vertices[k].ToOpenTK().FixCoordSystem(),
                    Normal = headMesh.Normals[k].ToOpenTK().FixCoordSystem()
                };
            }

            _originalHeadVertices = headVertices;
            _headVertices = (PosNorm[])headVertices.Clone();

            for (var k = 0; k < headIndices.Length; ++k) {
                headIndices[k] = headMesh.Indices[k];
            }

            headVertexBuffer.BufferData(headVertices, BufferUsageHint.StreamDraw);
            headIndexBuffer.BufferData(headIndices, BufferUsageHint.StaticDraw);

            _headVertexBuffer = headVertexBuffer;
            _headIndexBuffer = headIndexBuffer;

            #endregion
        }

        public void Update() {
            if (!Enabled) {
                return;
            }

            UpdateVertices(_game.CurrentTime);

            _bodyVertexBuffer.BufferData(_bodyVertices, BufferUsageHint.StreamDraw);
            _headVertexBuffer.BufferData(_headVertices, BufferUsageHint.StreamDraw);
        }

        public void Draw() {
            if (!Visible) {
                return;
            }

            foreach (var subMesh in _bodyMesh.SubMeshes) {
                DrawBuffered(_bodyVertexBuffer, _bodyIndexBuffer, subMesh.FirstIndex, subMesh.IndexCount * 3);
            }

            foreach (var subMesh in _headMesh.SubMeshes) {
                DrawBuffered(_headVertexBuffer, _headIndexBuffer, subMesh.FirstIndex, subMesh.IndexCount * 3);
            }
        }

        public bool Enabled { get; set; } = true;

        public bool Visible { get; set; } = true;

        protected override void Dispose(bool disposing) {
            _bodyVertexBuffer?.Dispose();
            _bodyVertexBuffer = null;

            _bodyIndexBuffer?.Dispose();
            _bodyIndexBuffer = null;

            _headVertexBuffer?.Dispose();
            _headVertexBuffer = null;

            _headIndexBuffer?.Dispose();
            _headIndexBuffer = null;

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

        private void UpdateVertices(float currentTime) {
            var bodyMesh = _bodyMesh;
            var bodyAvatar = _bodyAvatar;
            var bodyVertices = _bodyVertices;
            var headMesh = _headMesh;
            var headAvatar = _headAvatar;
            var origBodyVertices = _originalBodyVertices;
            var headVertices = _headVertices;
            var origHeadVertices = _originalHeadVertices;

            var keyFrames = _animation.KeyFrames;

#if !STATIC_POSE_DEBUG
            var animatedBoneCount = _animation.BoneCount;
            var firstKeyFrameIndex = -1;
            var lastFirstKeyFrameIndex = 0;

            for (var i = 0; i < keyFrames.Length; i += animatedBoneCount) {
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
                var bone = _bodyBoneList.FirstOrDefault(b => b.Path == altPath);

                if (bone == null) {
                    throw new ApplicationException($"Invalid bone path: {altPath}");
                }

                BoneNode transferredBone = null;

                foreach (var kv in BoneAttachmentMap) {
                    if (kv.Key == altPath) {
                        transferredBone = _headBoneList.SingleOrDefault(b => b.Path == kv.Value);

                        if (transferredBone == null) {
                            throw new ArgumentException();
                        }

                        break;
                    }
                }

                if (frame.HasPositions) {
                    var x = frame.PositionX.Value;
                    var y = frame.PositionY.Value;
                    var z = frame.PositionZ.Value;

                    var t = new Vector3(x, y, z);

                    t = t.FixCoordSystem();

                    bone.LocalPosition = t;

                    //if (transferredBone != null) {
                    //    transferredBone.LocalPosition = t;
                    //}
                }

                if (frame.HasRotations) {
                    var q = UnityRotation.EulerDeg(frame.AngleX.Value, frame.AngleY.Value, frame.AngleZ.Value);

                    q = q.FixCoordSystem();

                    bone.LocalRotation = q;

                    if (transferredBone != null) {
                        transferredBone.LocalRotation = q;
                    }
                }
            }
#endif

            foreach (var bone in _bodyBoneList) {
                bone.UpdateTransform();
            }

            foreach (var bone in _headBoneList) {
                bone.UpdateTransform();
            }

            UpdateVertInternal(origBodyVertices, bodyVertices, bodyAvatar, bodyMesh, _bodyBoneList);
            UpdateVertInternal(origHeadVertices, headVertices, headAvatar, headMesh, _headBoneList);

            void UpdateVertInternal(PosNorm[] origVertices, PosNorm[] vertices, PrettyAvatar avatar, PrettyMesh mesh, IReadOnlyList<BoneNode> boneList) {
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
                        var transform = boneList[boneIndex].SkinMatrix;
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
            }

            ++_lastFrameIndex;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PosNorm {

            public Vector3 Position;

            public Vector3 Normal;

        }

        private static readonly IReadOnlyDictionary<string, string> BoneAttachmentMap = new Dictionary<string, string> {
            //["MODEL_00/BASE/MUNE1/MUNE2/KUBI"] = "KUBI",
            ["MODEL_00/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "KUBI/ATAMA"
        };

        private readonly PrettyMesh _bodyMesh;

        private readonly PrettyAvatar _bodyAvatar;

        private readonly IReadOnlyList<BoneNode> _bodyBoneList;

        private readonly PrettyMesh _headMesh;

        private readonly PrettyAvatar _headAvatar;

        private readonly IReadOnlyList<BoneNode> _headBoneList;

        private readonly BodyAnimation _animation;

        private readonly PosNorm[] _originalBodyVertices;

        private readonly PosNorm[] _bodyVertices;

        private VertexBuffer _bodyVertexBuffer;

        private IndexBuffer _bodyIndexBuffer;

        private readonly PosNorm[] _originalHeadVertices;

        private readonly PosNorm[] _headVertices;

        private VertexBuffer _headVertexBuffer;

        private IndexBuffer _headIndexBuffer;

        private int _lastFrameIndex = -1;

        private readonly Game _game;

    }
}
