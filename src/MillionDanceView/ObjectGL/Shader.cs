using System;
using System.Diagnostics;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;

namespace OpenMLTD.MillionDance.Viewer.ObjectGL {
    public sealed class Shader : DisposableBase {

        private Shader(int shader, ShaderType shaderType) {
            _shader = shader;
            ShaderType = shaderType;
        }

        public ShaderType ShaderType { get; }

        public int ObjectId => _shader;

        [NotNull]
        public string GetSource() {
            EnsureNotDisposed();

            GL.GetShader(_shader, ShaderParameter.ShaderSourceLength, out var length);
            GL.GetShaderSource(_shader, length, out var realLength, out var source);

#if DEBUG
            Debug.Print("Real shader source length: {0}", realLength);
#endif

            return source;
        }

        [NotNull]
        public static Shader Compile([NotNull] string source, ShaderType shaderType) {
            var shader = GL.CreateShader(shaderType);

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);

            if (status == 0) {
                var infoLog = GL.GetShaderInfoLog(shader);

                GL.DeleteShader(shader);

                var errorMessage = "Failed to compile shader:" + Environment.NewLine + infoLog;

                throw new OpenGLException(errorMessage);
            }

            return new Shader(shader, shaderType);
        }

        protected override void Dispose(bool disposing) {
            GL.DeleteShader(_shader);
            _shader = 0;

            base.Dispose(disposing);
        }

        private int _shader;

    }
}
