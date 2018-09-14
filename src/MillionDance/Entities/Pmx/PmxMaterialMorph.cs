using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxMaterialMorph : PmxBaseMorph {

        internal PmxMaterialMorph() {
        }

        public MorphOp Op { get; internal set; }

        public Vector4 Diffuse { get; internal set; }

        public Vector3 Specular { get; internal set; }

        public float SpecularPower { get; internal set; }

        public Vector3 Ambient { get; internal set; }

        public float EdgeSize { get; internal set; }

        public Vector4 EdgeColor { get; internal set; }

        public Vector4 Texture { get; internal set; }

        public Vector4 Sphere { get; internal set; }

        public Vector4 Toon { get; internal set; }

    }
}
