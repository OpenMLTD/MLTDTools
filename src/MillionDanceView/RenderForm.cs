using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using AssetStudio.Extended.CompositeModels;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Mltd;
using OpenMLTD.MillionDance.Viewer.Internal;
using OpenMLTD.MillionDance.Viewer.ObjectGL;
using OpenMLTD.MillionDance.Viewer.Programs;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenMLTD.MillionDance.Viewer {
    public sealed class RenderForm : GameWindow {

        public RenderForm() {
            _game = new Game();
            RegisterEventHandlers();

            ClientSize = new Size(512, 512);
        }

        ~RenderForm() {
            UnregisterEventHandlers();
        }

        private void UnregisterEventHandlers() {
            Load -= RenderForm_Load;
            UpdateFrame -= RenderForm_UpdateFrame;
            RenderFrame -= RenderForm_RenderFrame;
            Closed -= RenderForm_Closed;
            KeyDown -= RenderForm_KeyDown;
            MouseDown -= RenderForm_MouseDown;
            MouseUp -= RenderForm_MouseUp;
            Resize -= RenderForm_Resize;
        }

        private void RegisterEventHandlers() {
            Load += RenderForm_Load;
            UpdateFrame += RenderForm_UpdateFrame;
            RenderFrame += RenderForm_RenderFrame;
            Closed += RenderForm_Closed;
            KeyDown += RenderForm_KeyDown;
            MouseDown += RenderForm_MouseDown;
            MouseUp += RenderForm_MouseUp;
            Resize += RenderForm_Resize;
        }

        private void RenderForm_Resize(object sender, EventArgs e) {
            GL.Viewport(ClientSize);
        }

        private void RenderForm_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) {
                _trackingMouse = false;
            }
        }

        private void RenderForm_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) {
                _trackingMouse = true;

                var mouseState = Mouse.GetState();
                _lastMousePos = new Vector2(mouseState.X, mouseState.Y);
            }
        }

        private void RenderForm_KeyDown(object sender, KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.Space:
                    _animationRenderer.Enabled = !_animationRenderer.Enabled;
                    break;
                case Key.X:
                    _invertMouseX = !_invertMouseX;
                    break;
                case Key.Y:
                    _invertMouseY = !_invertMouseY;
                    break;
            }
        }

        private void RenderForm_Closed(object sender, EventArgs e) {
            _bodyBoneDebugger?.Dispose();
            _bodyBoneDebugger = null;

            _headBoneDebugger?.Dispose();
            _headBoneDebugger = null;

            _animationRenderer?.Dispose();
            _animationRenderer = null;

            _axesDebugger?.Dispose();
            _axesDebugger = null;

            _phongProgram?.Dispose();
            _phongProgram = null;

            _simpleColorProgram?.Dispose();
            _simpleColorProgram = null;

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

            if (_trackingMouse) {
                var mouseState = Mouse.GetState();
                var mousePos = new Vector2(mouseState.X, mouseState.Y);
                var delta = _lastMousePos - mousePos;

                if (_invertMouseX) {
                    delta.X = -delta.X;
                }

                if (_invertMouseY) {
                    delta.Y = -delta.Y;
                }

                _camera.AddRotation(delta.X, delta.Y);

                _lastMousePos = mousePos;
            }

            do {
                if (keyboardState.IsKeyDown(Key.W)) {
                    _camera.Move(0f, 0.1f, 0f);
                }

                if (keyboardState.IsKeyDown(Key.S)) {
                    _camera.Move(0f, -0.1f, 0f);
                }

                if (keyboardState.IsKeyDown(Key.A)) {
                    _camera.Move(-0.1f, 0f, 0f);
                }

                if (keyboardState.IsKeyDown(Key.D)) {
                    _camera.Move(0.1f, 0f, 0f);
                }

                if (keyboardState.IsKeyDown(Key.PageUp)) {
                    _camera.Move(0f, 0f, 0.1f);
                }

                if (keyboardState.IsKeyDown(Key.PageDown)) {
                    _camera.Move(0f, 0f, -0.1f);
                }
            } while (false);

            Update(_currentTime);

            _currentTime += (float)e.Time;
        }

        private void RenderForm_Load(object sender, EventArgs e) {
            _vertexArrayObject = new VertexArrayObject();

            Initialize();

            var thread = new Thread(Initialize1);

            thread.IsBackground = true;

            thread.Start();
        }

        private void Initialize() {
            _camera.Position = new Vector3(0, 1, 3);
            _camera.Up = new Vector3(0, 1, 0);
            _camera.LookAtTarget(new Vector3(0, 0.75f, 0));

            _phongProgram = ResHelper.LoadProgram<Phong>("Resources/Phong.vert", "Resources/Phong.frag");
            _simpleColorProgram = ResHelper.LoadProgram<SimpleColor>("Resources/SimpleColor.vert", "Resources/SimpleColor.frag");

            _phongProgram.LightColor = new Vector3(1, 1, 1);
            _phongProgram.Material = new Vector4(1, 1, 1, 0.5f);
            _phongProgram.Alpha = 1.0f;

            _phongProgram.LightPosition = new Vector3(1, 0, 5);

            GL.Enable(EnableCap.DepthTest);
        }

        // Some heavy stuff
        private void Initialize1() {
            ResHelper.LoadHeadMesh();

            _bodyMesh = ResHelper.LoadBodyMesh();
            _bodyAvatar = ResHelper.LoadBodyAvatar();
            _headMesh = ResHelper.LoadHeadMesh();
            _headAvatar = ResHelper.LoadHeadAvatar();

            Debug.Assert(_bodyAvatar != null, nameof(_bodyAvatar) + " != null");
            Debug.Assert(_bodyMesh != null, nameof(_bodyMesh) + " != null");
            Debug.Assert(_headAvatar != null, nameof(_headAvatar) + " != null");
            Debug.Assert(_headMesh != null, nameof(_headMesh) + " != null");

            _bodyBoneList = ResHelper.BuildBoneHierachy(_bodyAvatar, _bodyMesh);
            _headBoneList = ResHelper.BuildBoneHierachy(_headAvatar, _headMesh);

            do {
                // Fix "KUBI" (neck) bone's parent
                var kubiParent = _bodyBoneList.SingleOrDefault(bn => bn.Path == "MODEL_00/BASE/MUNE1/MUNE2/KUBI");
                var kubiBone = _headBoneList.SingleOrDefault(bn => bn.Path == "KUBI");

                Debug.Assert(kubiParent != null);
                Debug.Assert(kubiBone != null);

                kubiParent.AddChild(kubiBone);

                Debug.Assert(kubiBone.Parent != null);

                // Don't forget to remove it from its old parent (or it will be updated twice from two parents).
                // The original parents and grandparents of KUBI are not needed; they are just like model anchors and shouldn't be animated.
                // See decompiled model for more information.
                kubiBone.Parent.RemoveChild(kubiBone);

                kubiBone.Parent = kubiParent;

                // Set its new initial parameters.
                // Since the two bones (KUBI and MODEL_00/BASE/MUNE1/MUNE2/KUBI) actually share the exact same transforms,
                // set its local transform to identity (t=0, q=0).
                kubiBone.InitialPosition = Vector3.Zero;
                kubiBone.InitialRotation = Quaternion.Identity;
                kubiBone.LocalPosition = Vector3.Zero;
                kubiBone.LocalRotation = Quaternion.Identity;

                // Now inform the rest of the head bones the new bone hierarchy. Force them to do so.
                foreach (var bone in _headBoneList) {
                    bone.Initialize(true);
                }
            } while (false);

            // TODO: FIXME: new format is not supported here (it's hard-coded) but it is handled in MillionDance.
            (_danceData, _, _) = ResHelper.LoadDance();

#if DEBUG
            do {
                var influencingBones = _bodyBoneList.Where((_, i) => _bodyMesh.Skin.Any(sk => sk.Any(a => a.BoneIndex == i)));

                Debug.Print("Bones that influences the mesh:");

                foreach (var bone in influencingBones) {
                    Debug.Print("#{0} \"{1}\"", bone.Index, bone.Path);
                }
            } while (false);
#endif

            _animation = new LegacyBodyAnimationSource(_danceData).Convert();

#if DEBUG
            do {
                var animatedBoneNames = _animation.KeyFrames.Select(f => f.Path).Distinct();

                Debug.Print("Animated bone names:");

                foreach (var boneName in animatedBoneNames) {
                    Debug.Print(boneName);
                }
            } while (false);
#endif

            _initialized1 = true;
        }

        private void Initialize2() {
            _animationRenderer = new AnimationRenderer(_game, _bodyMesh, _bodyAvatar, _bodyBoneList, _headMesh, _headAvatar, _headBoneList, _animation);
            _bodyBoneDebugger = new BoneDebugger(_bodyBoneList, _simpleColorProgram) {
                Color = new Vector4(1, 0, 0, 0.5f)
            };
            _headBoneDebugger = new BoneDebugger(_headBoneList, _simpleColorProgram) {
                Color = new Vector4(0, 1, 0, 0.5f)
            };

            _animationRenderer.Enabled = false;

            _axesDebugger = new AxesDebugger(_simpleColorProgram);
        }

        private void Update(float currentTime) {
            if (!_initialized1) {
                return;
            }

            if (!_initialized2) {
                Initialize2();
                _initialized2 = true;
            }

            _game.CurrentTime = currentTime;

            var size = ClientSize;
            var aspect = (float)size.Width / size.Height;

            _camera.Update();

            _worldMatrix = Matrix4.Identity;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), aspect, 0.1f, 100.0f);

            _phongProgram.SetMatrices(_worldMatrix, _camera.ViewMatrix, _projectionMatrix);
            _simpleColorProgram.SetMatrices(_worldMatrix, _camera.ViewMatrix, _projectionMatrix);

            _phongProgram.ViewPosition = _camera.Position;

            _animationRenderer.Update();
            _bodyBoneDebugger.Update();
            _headBoneDebugger.Update();
        }

        private void Draw() {
            if (!_initialized2) {
                return;
            }

            GL.DepthFunc(DepthFunction.Less);

            // The animated model
            _phongProgram.Activate();
            _animationRenderer.Draw();

            // Axes (R=X, G=Y, B=Z)
            _simpleColorProgram.Activate();
            _axesDebugger.Draw();

            GL.DepthFunc(DepthFunction.Always);

            // Bone debugging info
            _simpleColorProgram.Activate();
            _bodyBoneDebugger.Draw();
            _headBoneDebugger.Draw();
        }

        private float _currentTime;

        private bool _initialized1;
        private bool _initialized2;

        private AnimationRenderer _animationRenderer;
        private BoneDebugger _bodyBoneDebugger;
        private BoneDebugger _headBoneDebugger;
        private AxesDebugger _axesDebugger;

        private CharacterImasMotionAsset _danceData;
        private BodyAnimation _animation;
        private PrettyAvatar _bodyAvatar;
        private PrettyMesh _bodyMesh;
        private IReadOnlyList<BoneNode> _bodyBoneList;
        private PrettyAvatar _headAvatar;
        private PrettyMesh _headMesh;
        private IReadOnlyList<BoneNode> _headBoneList;

        private Matrix4 _worldMatrix, _projectionMatrix;

        private Vector2 _lastMousePos;
        private bool _trackingMouse;
        private bool _invertMouseX;
        private bool _invertMouseY;

        private readonly Camera _camera = new Camera();

        private VertexArrayObject _vertexArrayObject;

        private Phong _phongProgram;
        private SimpleColor _simpleColorProgram;

        private readonly Game _game;

    }
}
