using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MillionDanceView.Extensions;
using MillionDanceView.ObjectGL;
using MillionDanceView.Specialized;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Unity;
using UnityStudio.Unity.Animation;
using Vector3 = OpenTK.Vector3;
using Vector4 = OpenTK.Vector4;

namespace MillionDanceView {
    public sealed class RenderForm : GameWindow {

        public RenderForm() {
            RegisterEventHandlers();
        }

        ~RenderForm() {
            UnregisterEventHandlers();
        }

        private void UnregisterEventHandlers() {
            Load -= RenderForm_Load;
            UpdateFrame -= RenderForm_UpdateFrame;
            RenderFrame -= RenderForm_RenderFrame;
            Closed -= RenderForm_Closed;
        }

        private void RegisterEventHandlers() {
            Load += RenderForm_Load;
            UpdateFrame += RenderForm_UpdateFrame;
            RenderFrame += RenderForm_RenderFrame;
            Closed += RenderForm_Closed;
        }

        private void RenderForm_Closed(object sender, EventArgs e) {
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;

            _indexBuffer?.Dispose();
            _indexBuffer = null;

            _program?.Dispose();
            _program = null;

            _vertexArrayObject?.Dispose();
            _vertexArrayObject = null;
        }

        private void RenderForm_RenderFrame(object sender, FrameEventArgs e) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Draw();

            SwapBuffers();
        }

        private void RenderForm_UpdateFrame(object sender, FrameEventArgs e) {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Key.Escape) || ((keyboardState.IsKeyDown(Key.AltLeft) || keyboardState.IsKeyDown(Key.AltRight)) && keyboardState.IsKeyDown(Key.F4))) {
                Exit();
            }

            Update();
        }

        private void RenderForm_Load(object sender, EventArgs e) {
            _vertexArrayObject = new VertexArrayObject();

            Initialize();
        }

        private void Initialize() {
            //_camera.Position = new Vector3(4, 3, 3);
            _camera.Position = new Vector3(0, 1, 2);
            _camera.Target = new Vector3(0, 0.75f, 0);
            _camera.Up = new Vector3(0, 1, 0);

            var vertSource = File.ReadAllText("Resources/Phong.vert");
            var fragSource = File.ReadAllText("Resources/Phong.frag");

            var vert = Shader.Compile(vertSource, ShaderType.VertexShader);
            var frag = Shader.Compile(fragSource, ShaderType.FragmentShader);

            var program = Program.Link<Phong>(vert, frag);

            _program = program;

            _mesh = LoadMesh();

            {
                var vertexBuffer = new VertexBuffer();
                var indexBuffer = new IndexBuffer();

                var vertices = new PosNorm[_mesh.Vertices.Length];
                var indices = new uint[_mesh.Indices.Count];

                for (var k = 0; k < vertices.Length; ++k) {
                    vertices[k] = new PosNorm {
                        Position = _mesh.Vertices[k].ToOpenTK(),
                        Normal = _mesh.Normals[k].ToOpenTK()
                    };
                }

                for (var k = 0; k < indices.Length; ++k) {
                    indices[k] = _mesh.Indices[k];
                }

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                vertexBuffer.BufferData(vertices, BufferUsageHint.StaticDraw);
                indexBuffer.BufferData(indices, BufferUsageHint.StaticDraw);

                _vertexBuffer = vertexBuffer;
                _indexBuffer = indexBuffer;
            }

            _avatar = LoadAvatar();

            _program.LightColor = new Vector3(1, 1, 1);
            _program.Material = new Vector4(1, 1, 1, 0.5f);
            _program.Alpha = 1.0f;

            _program.LightPosition = new Vector3(1, 0, 5);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            //GL.Disable(EnableCap.CullFace);
        }

        private void Update() {
            var size = ClientSize;
            var aspect = (float)size.Width / size.Height;

            _camera.Update();

            _worldMatrix = Matrix4.Identity;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), aspect, 0.1f, 100.0f);

            _program.SetMatrices(_worldMatrix, _camera.ViewMatrix, _projectionMatrix);

            _program.ViewPosition = _camera.Position;
        }

        private void Draw() {
            _program.Activate();

            foreach (var subMesh in _mesh.SubMeshes) {
                DrawBuffered(_vertexBuffer, _indexBuffer, subMesh.FirstIndex, subMesh.IndexCount * 3);
            }
        }

        private static void DrawBuffered(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, uint startIndex, uint elementCount) {
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

        private static Mesh LoadMesh() {
            Mesh mesh = null;

            using (var fileStream = File.Open(BodyModelFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Mesh) {
                                continue;
                            }

                            mesh = preloadData.LoadAsMesh();
                            break;
                        }
                    }
                }
            }

            return mesh;
        }

        private static Avatar LoadAvatar() {
            Avatar avatar = null;

            using (var fileStream = File.Open(BodyModelFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Avatar) {
                                continue;
                            }

                            avatar = preloadData.LoadAsAvatar();
                            break;
                        }
                    }
                }
            }

            return avatar;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PosNorm {

            public Vector3 Position;

            public Vector3 Normal;

        }

        private const string BodyModelFilePath = "Resources/cb_ss001_015siz.unity3d";

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private Avatar _avatar;
        private Mesh _mesh;

        private Matrix4 _worldMatrix, _projectionMatrix;

        private readonly Camera _camera = new Camera();

        private VertexArrayObject _vertexArrayObject;
        private Phong _program;

    }
}
