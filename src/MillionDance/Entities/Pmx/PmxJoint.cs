using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxJoint : IPmxNamedObject {

        internal PmxJoint() {
        }

        public string Name { get; internal set; } = string.Empty;

        public string NameEnglish { get; internal set; } = string.Empty;

        public JointKind Kind { get; internal set; }

        public int BodyIndex1 { get; internal set; }

        public int BodyIndex2 { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 Rotation { get; internal set; }

        public Vector3 LowerTranslationLimit { get; internal set; }

        public Vector3 UpperTranslationLimit { get; internal set; }

        public Vector3 LowerRotationLimit { get; internal set; }

        public Vector3 UpperRotationLimit { get; internal set; }

        public Vector3 TranslationSpringConstants { get; internal set; }

        public Vector3 RotationSpringConstants { get; internal set; }

    }
}
