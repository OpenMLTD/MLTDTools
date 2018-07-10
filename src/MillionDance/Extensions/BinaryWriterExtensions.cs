using System.IO;
using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Extensions {
    internal static class BinaryWriterExtensions {

        public static void Write([NotNull] this BinaryWriter writer, Vector3 vector) {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static void Write([NotNull] this BinaryWriter writer, Quaternion quaternion) {
            writer.Write(quaternion.X);
            writer.Write(quaternion.Y);
            writer.Write(quaternion.Z);
            writer.Write(quaternion.W);
        }

    }
}
