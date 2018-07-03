using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace PlatiumTheater.Extensions {
    public static class BinaryReaderExtensions {

        public static Vector3 ReadVector3([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();

            return new Vector3(x, y, z);
        }

        public static Vector4 ReadVector4([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();

            return new Vector4(x, y, z, w);
        }

        public static Quaternion ReadQuaternion([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();

            return new Quaternion(x, y, z, w);
        }

    }
}
