using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class RawTransform {

        public RawTransform() {
            Translation = Vector3.Zero;
            Rotation = IdentityRotation;
            Scale = Vector3.Zero;
        }

        internal RawTransform([NotNull] xform transform) {
            Translation = transform.t;
            Rotation = transform.q;
            Scale = transform.s;
        }

        public Vector3 Translation { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }

        private static readonly Quaternion IdentityRotation = new Quaternion(0, 0, 0, 1);

    }
}
