using MillionDanceView.ObjectGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MillionDanceView.Specialized {
    public sealed class Phong : Program {

        private Phong() {
        }

        public float Alpha {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_Alpha);
                var buf = new float[1];

                GL.GetUniform(ObjectId, loc, buf);

                return buf[0];
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_Alpha);

                GL.Uniform1(loc, value);
            }
        }

        public Vector3 LightPosition {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_LightPosition);
                var buf = new float[3];

                GL.GetUniform(ObjectId, loc, buf);

                return new Vector3(buf[0], buf[1], buf[2]);
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_LightPosition);

                GL.Uniform3(loc, ref value);
            }
        }

        public Vector3 LightColor {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_LightColor);
                var buf = new float[3];

                GL.GetUniform(ObjectId, loc, buf);

                return new Vector3(buf[0], buf[1], buf[2]);
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_LightColor);

                GL.Uniform3(loc, ref value);
            }
        }

        public Vector3 ViewPosition {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_ViewPosition);
                var buf = new float[3];

                GL.GetUniform(ObjectId, loc, buf);

                return new Vector3(buf[0], buf[1], buf[2]);
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_ViewPosition);

                GL.Uniform3(loc, ref value);
            }
        }

        public Vector4 Material {
            get {
                Activate();

                var loc = GetUniformLocation(Uniform_ObjectColor);
                var buf = new float[4];

                GL.GetUniform(ObjectId, loc, buf);

                return new Vector4(buf[0], buf[1], buf[2], buf[3]);
            }
            set {
                Activate();

                var loc = GetUniformLocation(Uniform_ObjectColor);

                GL.Uniform4(loc, ref value);
            }
        }

        public void SetMatrices(Matrix4 world, Matrix4 view, Matrix4 projection) {
            EnsureNotDisposed();

            Activate();

            var wvp = world * view * projection;
            var loc = GetUniformLocation(Uniform_WorldViewProjection);
            GL.UniformMatrix4(loc, false, ref wvp);

            var wti = Matrix4.Transpose(world.Inverted());
            loc = GetUniformLocation(Uniform_WorldTransposedInverted);
            GL.UniformMatrix4(loc, false, ref wti);

            loc = GetUniformLocation(Uniform_World);
            GL.UniformMatrix4(loc, false, ref world);
        }

        // ReSharper disable once InconsistentNaming
        private const string Uniform_WorldViewProjection = "uWVP";
        private const string Uniform_World = "uW";
        private const string Uniform_WorldTransposedInverted = "uWti";
        private const string Uniform_LightPosition = "uLightPos";
        private const string Uniform_LightColor = "uLightColor";
        private const string Uniform_ObjectColor = "uObjectColor";
        private const string Uniform_ViewPosition = "uViewPos";
        private const string Uniform_Alpha = "uAlpha";

    }
}
