using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.ObjectGL {
    public class Program : DisposableBase, IGLObject {

        protected Program() {
        }

        public void Activate() {
            EnsureNotDisposed();

            GL.UseProgram(_program);
        }

        public Shader VertexShader => _vertexShader;

        public Shader FragmentShader => _fragmentShader;

        [NotNull]
        public static T Link<T>([CanBeNull] Shader vertexShader, [CanBeNull] Shader fragmentShader)
            where T : Program {
            var program = GL.CreateProgram();

            if (vertexShader == null && fragmentShader == null) {
                throw new InvalidOperationException("At least one of vertex shader and fragment shader should exist.");
            }

            if (vertexShader != null) {
                GL.AttachShader(program, vertexShader.ObjectId);
            }

            if (fragmentShader != null) {
                GL.AttachShader(program, fragmentShader.ObjectId);
            }

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);

            if (status == 0) {
                var infoLog = GL.GetProgramInfoLog(program);

                if (vertexShader != null) {
                    GL.DetachShader(program, vertexShader.ObjectId);
                }

                if (fragmentShader != null) {
                    GL.DetachShader(program, fragmentShader.ObjectId);
                }

                GL.DeleteProgram(program);

                var errorMessage = "Failed to link program:" + Environment.NewLine + infoLog;

                throw new OpenGLException(errorMessage);
            }

            if (vertexShader != null) {
                GL.DetachShader(program, vertexShader.ObjectId);
            }

            if (fragmentShader != null) {
                GL.DetachShader(program, fragmentShader.ObjectId);
            }

            var t = typeof(T);

            const BindingFlags allInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var ctor = t.GetConstructor(allInstance, null, NoParameter, NoModifier);

            if (ctor == null) {
                throw new MissingMethodException(t.FullName, "Missing parameterless constructor.");
            }

            var p = (T)ctor.Invoke(EmptyObjectArray);

            p._program = program;
            p._vertexShader = vertexShader;
            p._fragmentShader = fragmentShader;

            return p;
        }

        public int ObjectId => _program;

        public int GetAttributeLocation([NotNull] string name) {
            EnsureNotDisposed();

            if (_attributeLocations.ContainsKey(name)) {
                return _attributeLocations[name];
            }

            var location = GL.GetAttribLocation(_program, name);

            if (location >= 0) {
                _attributeLocations[name] = location;
            }

            return location;
        }

        public int GetUniformLocation([NotNull] string name) {
            EnsureNotDisposed();

            if (_uniformLocations.ContainsKey(name)) {
                return _uniformLocations[name];
            }

            var location = GL.GetUniformLocation(_program, name);

            if (location >= 0) {
                _uniformLocations[name] = location;
            }

            return location;
        }

        protected override void Dispose(bool disposing) {
            GL.DeleteProgram(_program);
            _program = 0;

            _vertexShader?.Dispose();
            _fragmentShader?.Dispose();

            _vertexShader = null;
            _fragmentShader = null;

            base.Dispose(disposing);
        }

        private readonly Dictionary<string, int> _attributeLocations = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

        private static readonly Type[] NoParameter = new Type[0];
        private static readonly ParameterModifier[] NoModifier = new ParameterModifier[0];
        private static readonly object[] EmptyObjectArray = new object[0];

        private int _program;
        private Shader _vertexShader;
        private Shader _fragmentShader;

    }
}
