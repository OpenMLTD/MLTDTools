using AssetStudio.Extended.MonoBehaviours.Serialization;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;

namespace Imas.Data.Serialized {
    [ScriptableObject(NamingConventionType = typeof(CamelCaseNamingConvention))]
    // ReSharper disable once InconsistentNaming
    public struct Vector4f {

        public float X;

        public float Y;

        public float Z;

        public float W;

        public override string ToString() {
            return $"Vector4f ({X}, {Y}, {Z}, {W})";
        }

    }
}
