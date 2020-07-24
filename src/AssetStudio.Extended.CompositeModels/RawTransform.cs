using System;
using AssetStudio.Extended.CompositeModels.Utilities;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class RawTransform : ICloneable {

        public RawTransform()
            : this(Vector3.Zero, Quaternions.Identity, Vector3.Zero) {
        }

        public RawTransform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale) {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
            LocalScale = localScale;
        }

        internal RawTransform([NotNull] xform transform) {
            LocalPosition = transform.t;
            LocalRotation = transform.q;
            LocalScale = transform.s;
        }

        internal RawTransform([NotNull] Transform transform) {
            LocalPosition = transform.m_LocalPosition;
            LocalRotation = transform.m_LocalRotation;
            LocalScale = transform.m_LocalScale;
        }

        public Vector3 LocalPosition { get; set; }

        public Quaternion LocalRotation { get; set; }

        public Vector3 LocalScale { get; set; }

        public RawTransform Clone() {
            return new RawTransform(LocalPosition, LocalRotation, LocalScale);
        }

        object ICloneable.Clone() {
            return Clone();
        }

    }
}
